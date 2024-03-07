using AutoMapper;
using Domain.Controller;
using Domain.Dtos.AccountingService;
using Domain.Dtos.CategoryService;
using Domain.Dtos.IdentityPlatform;
using Domain.Model;
using Domain.Services;
using HomeAccountant.AccountingService.Data;
using HomeAccountant.AccountingService.Models;
using HomeAccountant.AccountingService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
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
        private readonly IAccountInfoService _accountInfoService;
        private readonly ILogger<EntryController> _logger;

        public EntryController(IRepository<ApplicationDbContext, Entry> entryRepository,
            IRepository<ApplicationDbContext, Register> registerRepository,
            IMapper mapper,
            ICategoriesService categoriesService,
            IAccountInfoService accountInfoService,
            ILogger<EntryController> logger)
        {
            _repository = entryRepository;
            _registerRepository = registerRepository;
            _mapper = mapper;
            _categoriesService = categoriesService;
            _accountInfoService = accountInfoService;
            _logger = logger;
        }

        [HttpGet("{entryId}", Name = "GetEntryById")]
        public async Task<ActionResult<ServiceResponse<EntryReadDto>>> GetEntryById(int entryId)
        {
            var entryModel = _repository.Get(x => x.Id == entryId);

            if (entryModel is null)
                return NotFound(new ServiceResponse("Entry not found"));

            var categoryModel = await _categoriesService.GetCategoryAsync(entryModel.CategoryId);

            if (!categoryModel.Result)
                return NotFound(
                    new ServiceResponse()
                    {
                        Result = categoryModel.Result,
                        Errors = categoryModel.Errors
                    });

            var entryRead = _mapper.Map<EntryReadDto>(entryModel);
            entryRead.Category = categoryModel.Value;

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
                    .OrderByDescending(x => x.CreatedDate)
                    .Chunk(recordsOnPage);

                if (paggedEntryModels is null)
                    return NotFound(new ServiceResponse("Entries not found"));

                var entryModels = paggedEntryModels
                    .Skip(currentPage)
                    .FirstOrDefault();

                if (entryModels is null)
                    return NotFound(new ServiceResponse("Entries not found"));

                var userData = await GetUserData(
                    entryModels
                    .Select(x => x.CreatedBy)
                    .Distinct()
                    .ToArray());

                List<EntryReadDto> result = new List<EntryReadDto>();

                foreach (var item in entryModels)
                {
                    var model = _mapper.Map<EntryReadDto>(item);
                    var categoryModel = await _categoriesService.GetCategoryAsync(item.CategoryId);

                    model.Category = _mapper.Map<CategoryReadDto>(categoryModel.Value);
                    model.Creator = userData?.FirstOrDefault(x => x.UserId == item.CreatedBy)?.UserName ?? string.Empty;
                    result.Add(model);
                }

                return Ok(new ServiceResponse<PaggedResult<EntryReadDto>>(new PaggedResult<EntryReadDto>(result, page, paggedEntryModels.Count())));
            }
            catch (Exception ex)
            {
                return BadRequest(new ServiceResponse(ex.Message));
            }
        }

        private async Task<UserUsernameReadDto[]?> GetUserData(string[] userIds)
        {
            var userInfoResponse = await _accountInfoService.GetUsersData(userIds);

            if (!userInfoResponse.Result)
                return null;

            return userInfoResponse.Value;
        }

        [HttpPost]
        public async Task<ActionResult<ServiceResponse<EntryReadDto>>> Add(int registerId, int billingPeriodId, EntryCreateDto entryCreateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new ServiceResponse("Invalid payload"));

                if (UserId is null)
                    return BadRequest(new ServiceResponse("Missing UserId"));

                if (!CheckIfRegisterExists(registerId))
                    return NotFound(new ServiceResponse("Register doesn't exist"));

                if (!await _categoriesService.CategoryExists(entryCreateDto.CategoryId))
                    return NotFound(new ServiceResponse("Category not found"));

                var entryModel = _mapper.Map<Entry>(entryCreateDto);
                entryModel.CreatedBy = UserId;
                entryModel.BillingPeriodId = billingPeriodId;
                _repository.Add(entryModel);
                await _repository.SaveChangesAsync();

                var categoryModel = await _categoriesService.GetCategoryAsync(entryModel.CategoryId);

                if (!categoryModel.Result)
                    return NotFound(
                        new ServiceResponse<EntryReadDto>(
                            categoryModel.Errors ?? new string[] { "Reading category failed" }));

                var entryModelRead = _mapper.Map<EntryReadDto>(entryModel);
                entryModelRead.Category = categoryModel.Value;

                return CreatedAtRoute(
                    nameof(GetEntryById),
                    new
                    {
                        registerId,
                        billingPeriodId,
                        entryId = entryModelRead.Id
                    },
                    new ServiceResponse<EntryReadDto>(entryModelRead));
            }
            catch (Exception ex)
            {
                _logger.LogError($"--> {ex.Message}");

                return BadRequest(
                    new ServiceResponse<EntryReadDto>(
                        new string[] 
                        {
                            "Adding entry failed"
                        }));
            }
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

            if (UserId is null)
                return BadRequest(new ServiceResponse("Invalid payload"));

            if (!await _categoriesService.CategoryExists(entryUpdateDto.CategoryId))
                return NotFound(new ServiceResponse("Category not found"));

            entryModel.CreatedBy = UserId;
            entryModel.BillingPeriodId = billingPeriodId;
            _repository.Update(entryModel);
            await _repository.SaveChangesAsync();

            return Ok(new ServiceResponse(true));
        }
    }
}
