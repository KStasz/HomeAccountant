using AutoMapper;
using Domain.Controller;
using Domain.Dtos.AccountingService;
using Domain.Dtos.CategoryService;
using Domain.Model;
using Domain.Services;
using HomeAccountant.AccountingService.Data;
using HomeAccountant.AccountingService.Models;
using HomeAccountant.AccountingService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace HomeAccountant.AccountingService.Controllers
{
    [Route("api/Register/{registerId}/BillingPeriod/{billingPeriodId}/[controller]")]
    [ApiController]
    [Authorize]
    public class EntryController : ServiceControllerBase
    {
        private readonly IRepository<ApplicationDbContext, Entry> _repository;
        private readonly IRepository<ApplicationDbContext, Register> _registerRepository;
        private readonly IMapper _mapper;
        private readonly ICategoriesService _categoriesService;

        public EntryController(IRepository<ApplicationDbContext, Entry> entryRepository,
            IRepository<ApplicationDbContext, Register> registerRepository,
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
        public async Task<ActionResult<ServiceResponse<PaggedResult<EntryReadDto>>>> Get(int billingPeriodId, [Required] int page = 1, int recordsOnPage = 10)
        {
            try
            {
                var currentPage = page - 1;
                var paggedEntryModels = _repository
                    .GetAll(
                        x => x.BillingPeriodId == billingPeriodId,
                        x => x.BillingPeriod)
                    .Chunk(recordsOnPage);

                if (paggedEntryModels is null)
                    return NotFound(new ServiceResponse("Entries not found"));

                var entryModels = paggedEntryModels
                    .Skip(currentPage)
                    .FirstOrDefault();

                if (entryModels is null)
                    return NotFound(new ServiceResponse("Entries not found"));

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

                return Ok(new ServiceResponse<PaggedResult<EntryReadDto>>(new PaggedResult<EntryReadDto>(result, page, paggedEntryModels.Count())));
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
