using AutoMapper;
using Domain.Dtos.AccountingService;
using HomeAccountant.AccountingService.Models;
using HomeAccountant.AccountingService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace HomeAccountant.AccountingService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RegisterController : ServiceControllerBase
    {
        private readonly IRepository<Register> _repository;
        private readonly IMapper _mapper;
        private readonly ICategoriesService _categoriesService;
        private readonly IRepository<Entry> _entryRepository;

        public RegisterController(IRepository<Register> repository,
            IMapper mapper,
            ICategoriesService categoriesService,
            IRepository<Entry> entryRepository)
        {
            _repository = repository;
            _mapper = mapper;
            _categoriesService = categoriesService;
            _entryRepository = entryRepository;
        }

        [HttpPost]
        public async Task<IActionResult> Add(RegisterCreateDto registerCreateDto)
        {
            var userId = GetUserId();

            if (!ModelState.IsValid || userId is null)
                return BadRequest("Invalid payload");

            var registerModel = _mapper.Map<Register>(registerCreateDto);
            registerModel.CreatorId = userId;
            _repository.Add(registerModel);
            await _repository.SaveChangesAsync();

            var response = _mapper.Map<RegisterReadDto>(registerModel);

            return CreatedAtRoute(nameof(GetRegisterById), new { id = response.Id }, response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var registerModel = _repository.Get(x => x.Id == id);

            if (registerModel is null)
                return NotFound();

            _repository.Remove(registerModel);
            await _repository.SaveChangesAsync();
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Update(RegisterUpdateDto registerUpdateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid payload");

            var userId = GetUserId();

            if (userId is null)
                return BadRequest("Invalid payload");

            var registerModel = _mapper.Map<Register>(registerUpdateDto);
            registerModel.CreatorId = userId;

            EntityEntry entity = _repository.Update(registerModel);
            entity.Property(nameof(Register.CreatedDate)).IsModified = false;
            await _repository.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("Entries/{categoryId}")]
        public async Task<IActionResult> DeleteEntriesByCategoryId(int categoryId)
        {
            var userId = GetUserId();

            if (userId is null)
                return BadRequest("Invalid payload");

            var entries = _entryRepository.GetAll(x => x.CategoryId == categoryId);

            if (!entries.Any())
            {
                return NotFound();
            }

            _entryRepository.RemoveMany(entries);
            await _entryRepository.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("{id}", Name = "GetRegisterById")]
        public IActionResult GetRegisterById(int id)
        {
            var registerModel = _repository.Get(x => x.Id == id);
            var userId = GetUserId();

            if (registerModel is null)
                return NotFound();

            if (userId is null)
                return BadRequest("Invalid payload");

            if (registerModel.CreatorId != userId)
                return Unauthorized("You don't have access to this resource");

            var registerResponse = _mapper.Map<RegisterReadDto>(registerModel);

            return Ok(registerResponse);
        }

        [HttpGet]
        public IActionResult Get()
        {
            var userId = GetUserId();

            if (userId is null)
                return BadRequest("Invalid payload");

            var registers = _repository.GetAll(x => x.CreatorId == userId).OrderByDescending(x => x.CreatedDate);

            return Ok(_mapper.Map<IEnumerable<RegisterReadDto>>(registers));
        }
    }
}
