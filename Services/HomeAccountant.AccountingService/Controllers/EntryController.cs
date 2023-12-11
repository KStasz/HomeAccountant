using AutoMapper;
using Domain.Dtos.AccountingService;
using Domain.Dtos.CategoryService;
using HomeAccountant.AccountingService.Models;
using HomeAccountant.AccountingService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace HomeAccountant.AccountingService.Controllers
{
    [Route("api/Register/{registerId}/[controller]")]
    [ApiController]
    [Authorize]
    public class EntryController : ServiceControllerBase
    {
        private readonly IRepository<Entry> _repository;
        private readonly IRepository<Register> _registerRepository;
        private readonly IMapper _mapper;
        private readonly ICategoriesService _categoriesService;

        public EntryController(IRepository<Entry> entryRepository,
            IRepository<Register> registerRepository,
            IMapper mapper,
            ICategoriesService categoriesService)
        {
            _repository = entryRepository;
            _registerRepository = registerRepository;
            _mapper = mapper;
            _categoriesService = categoriesService;
        }

        [HttpGet("{entryId}", Name = "GetEntryById")]
        public async Task<IActionResult> GetEntryById(int entryId)
        {
            var entryModel = _repository.Get(x => x.Id == entryId);

            if (entryModel is null)
                return NotFound();

            var categoryModel = await _categoriesService.GetCategoryAsync(entryModel.CategoryId);
            var registerModel = _registerRepository.Get(x => x.Id == entryModel.RegisterId);

            if (categoryModel is null)
                return NotFound();

            if (registerModel is null)
                return NotFound();

            var entryRead = _mapper.Map<EntryReadDto>(entryModel);
            var registerRead = _mapper.Map<RegisterReadDto>(registerModel);

            entryRead.Category = categoryModel;
            entryRead.Register = registerRead;

            return Ok(entryRead);
        }

        [HttpGet]
        public async Task<IActionResult> Get(int registerId)
        {
            try
            {

                var entryModels = _repository.GetAll(x => x.RegisterId == registerId).ToList();

                List<EntryReadDto> result = new List<EntryReadDto>();

                foreach (var item in entryModels)
                {
                    var model = _mapper.Map<EntryReadDto>(item);
                    var categoryModel = await _categoriesService.GetCategoryAsync(item.CategoryId);
                    var registerModel = _registerRepository.Get(x => x.Id == item.RegisterId);

                    if (categoryModel is null)
                        continue;

                    if (registerModel is null)
                        continue;

                    model.Category = _mapper.Map<CategoryReadDto>(categoryModel);
                    model.Register = _mapper.Map<RegisterReadDto>(registerModel);
                    result.Add(model);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add(int registerId, EntryCreateDto entryCreateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid payload");

            var userId = GetUserId();

            if (userId is null)
                return BadRequest();

            if (!CheckIfRegisterExists(registerId))
                return NotFound();

            if (!await _categoriesService.CategoryExists(entryCreateDto.CategoryId))
                return NotFound("Category not found");

            var entryModel = _mapper.Map<Entry>(entryCreateDto);
            entryModel.CreatedBy = userId;
            entryModel.RegisterId = registerId;
            _repository.Add(entryModel);
            await _repository.SaveChangesAsync();

            var categoryModel = await _categoriesService.GetCategoryAsync(entryModel.CategoryId);

            var entryModelRead = _mapper.Map<EntryReadDto>(entryModel);
            entryModelRead.Category = _mapper.Map<CategoryReadDto>(categoryModel);

            return CreatedAtRoute(nameof(GetEntryById), new { registerId = registerId, entryId = entryModel.Id }, entryModelRead);
        }

        private bool CheckIfRegisterExists(int registerId)
        {
            var register = _registerRepository.Get(x => x.Id == registerId);

            return register is not null;
        }

        [HttpDelete("{entryId}")]
        public async Task<IActionResult> Delete(int entryId)
        {
            var entryModel = _repository.Get(x => x.Id == entryId);

            if (entryModel is null)
                return NotFound();

            _repository.Remove(entryModel);
            await _repository.SaveChangesAsync();

            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Update(int registerId, EntryUpdateDto entryUpdateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid payload");

            var entryModel = _mapper.Map<Entry>(entryUpdateDto);

            var userId = GetUserId();

            if (userId is null)
                return BadRequest("Invalid payload");

            if (!await _categoriesService.CategoryExists(entryUpdateDto.CategoryId))
                return NotFound("Category not found");

            entryModel.CreatedBy = userId;
            entryModel.RegisterId = registerId;
            _repository.Update(entryModel);
            await _repository.SaveChangesAsync();

            return Ok();
        }
    }
}
