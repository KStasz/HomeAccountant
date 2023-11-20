using AutoMapper;
using Domain.Dtos.AccountingService;
using HomeAccountant.AccountingService.Models;
using HomeAccountant.AccountingService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace HomeAccountant.AccountingService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RegisterController : ServiceControllerBase
    {
        private readonly IRepository<Register> _repository;
        private readonly IMapper _mapper;

        public RegisterController(IRepository<Register> repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
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
            entity.Property( nameof(Register.CreatedDate)).IsModified = false;
            await _repository.SaveChangesAsync();
            
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

            var registers = _repository.GetAll(x => x.CreatorId == userId);
            
            return Ok(_mapper.Map<IEnumerable<RegisterReadDto>>(registers));
        }
    }
}
