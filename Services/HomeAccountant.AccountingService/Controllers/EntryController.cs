using AutoMapper;
using Domain.Dtos;
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
    [Route("api/Register/{registerId}/BillingPeriod/{billingPeriodId}/[controller]")]
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
        public async Task<ActionResult<ServiceResponse<EntryReadDto>>> GetEntryById(int entryId)
        {
            var entryModel = _repository.Get(x => x.Id == entryId);

            if (entryModel is null)
                return NotFound(new ServiceResponse("Entry not found"));

            var categoryModel = await _categoriesService.GetCategoryAsync(entryModel.CategoryId);

            if (categoryModel is null)
                return NotFound(new ServiceResponse("Category not found"));

            var entryRead = _mapper.Map<EntryReadDto>(entryModel);
            entryRead.Category = categoryModel;

            return Ok(new ServiceResponse<EntryReadDto>(entryRead));
        }

        [HttpGet]
        public async Task<ActionResult<ServiceResponse<IEnumerable<EntryReadDto>>>> Get(int billingPeriodId)
        {
            try
            {
                var entryModels = _repository
                    .GetAll(
                        x => x.BillingPeriodId == billingPeriodId,
                        x => x.BillingPeriod)
                    .ToList();

                List<EntryReadDto> result = new List<EntryReadDto>();

                foreach (var item in entryModels)
                {
                    var model = _mapper.Map<EntryReadDto>(item);
                    var categoryModel = await _categoriesService.GetCategoryAsync(item.CategoryId);

                    if (categoryModel is null)
                        continue;

                    model.Category = _mapper.Map<CategoryReadDto>(categoryModel);
                    result.Add(model);
                }

                return Ok(new ServiceResponse<IEnumerable<EntryReadDto>>(result));
            }
            catch (Exception ex)
            {
                return BadRequest(new ServiceResponse(ex.Message));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ServiceResponse<EntryReadDto>>> Add(int registerId, int billingPeriodId, EntryCreateDto entryCreateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ServiceResponse("Invalid payload"));

            var userId = GetUserId();

            if (userId is null)
                return BadRequest(new ServiceResponse("Missing UserId"));

            if (!CheckIfRegisterExists(registerId))
                return NotFound(new ServiceResponse("Register doesn't exist"));

            if (!await _categoriesService.CategoryExists(entryCreateDto.CategoryId))
                return NotFound(new ServiceResponse("Category not found"));

            var entryModel = _mapper.Map<Entry>(entryCreateDto);
            entryModel.CreatedBy = userId;
            entryModel.BillingPeriodId = billingPeriodId;
            _repository.Add(entryModel);
            await _repository.SaveChangesAsync();

            var categoryModel = await _categoriesService.GetCategoryAsync(entryModel.CategoryId);

            var entryModelRead = _mapper.Map<EntryReadDto>(entryModel);
            entryModelRead.Category = _mapper.Map<CategoryReadDto>(categoryModel);

            return CreatedAtRoute(
                nameof(GetEntryById),
                new
                {
                    registerId = billingPeriodId,
                    entryId = entryModel.Id
                },
                new ServiceResponse<EntryReadDto>(entryModelRead));
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
                return NotFound(new ServiceResponse("Entry not found"));

            _repository.Remove(entryModel);
            await _repository.SaveChangesAsync();

            return Ok(new ServiceResponse(true));
        }

        [HttpPut]
        public async Task<ActionResult<ServiceResponse>> Update(int billingPeriodId, EntryUpdateDto entryUpdateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ServiceResponse("Invalid payload"));

            var entryModel = _mapper.Map<Entry>(entryUpdateDto);

            var userId = GetUserId();

            if (userId is null)
                return BadRequest(new ServiceResponse("Invalid payload"));

            if (!await _categoriesService.CategoryExists(entryUpdateDto.CategoryId))
                return NotFound(new ServiceResponse("Category not found"));

            entryModel.CreatedBy = userId;
            entryModel.BillingPeriodId = billingPeriodId;
            _repository.Update(entryModel);
            await _repository.SaveChangesAsync();

            return Ok(new ServiceResponse(true));
        }
    }
}
