using AutoMapper;
using Domain.Controller;
using Domain.Dtos.AccountingService;
using Domain.Model;
using Domain.Services;
using HomeAccountant.AccountingService.Data;
using HomeAccountant.AccountingService.Models;
using HomeAccountant.AccountingService.Services;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;

namespace HomeAccountant.AccountingService.Controllers
{
    [Route("api/Register/{registerId}/[controller]")]
    [ApiController]
    //[Authorize]
    public class BillingPeriodController : ServiceControllerBase
    {
        private readonly IRepository<ApplicationDbContext, BillingPeriod> _billingPeriodRepository;
        private readonly IRepository<ApplicationDbContext, Register> _registerRepository;
        private readonly IMapper _mapper;
        private readonly ICategoriesService _categoriesService;
        private Color[] _colorPallete = new Color[]
        {
            Color.Red,
            Color.Orange,
            Color.Yellow,
            Color.Green,
            Color.Blue,
            Color.Purple,
            Color.Magenta,
            Color.Pink
        };

        public BillingPeriodController(IRepository<ApplicationDbContext, BillingPeriod> billingPeriodRepository,
            IRepository<ApplicationDbContext, Register> registerRepository,
            IMapper mapper,
            ICategoriesService categoriesService)
        {
            _billingPeriodRepository = billingPeriodRepository;
            _registerRepository = registerRepository;
            _mapper = mapper;
            _categoriesService = categoriesService;
        }

        [HttpGet]
        public ActionResult<ServiceResponse<IEnumerable<BillingPeriodReadDto>>> GetAll(int registerId)
        {
            var result = _billingPeriodRepository
                .GetAll(
                    x => x.RegisterId == registerId,
                    x => x.Register);

            if (!result?.Any() ?? false)
            {
                return NotFound(new ServiceResponse(true));
            }

            var billingPeriodResponse = _mapper.Map<IEnumerable<BillingPeriodReadDto>>(result);

            return Ok(new ServiceResponse<IEnumerable<BillingPeriodReadDto>>(billingPeriodResponse));
        }

        [HttpGet("{billingPeriodId}/Statistic")]
        public async Task<ActionResult<ServiceResponse<EntriesStatisticDto>>> GetStatistic(int billingPeriodId)
        {
            var entryModels = _billingPeriodRepository
                .GetAll(
                    x => x.Id == billingPeriodId,
                    x => x.Entries);

            if (!entryModels.Any())
            {
                return NotFound();
            }

            var grouppedEntryModels = entryModels
                .SelectMany(x => x.Entries)
                .GroupBy(x => x.CategoryId)
                .Select((x, i) => new
                {
                    CategoryId = x.Key,
                    Sum = x.Sum(y => y.Price),
                    Color = _colorPallete[i]
                });

            var result = new EntriesStatisticDto();
            result.TotalSum = (double)entryModels.SelectMany(x => x.Entries.Select(y => y.Price)).Sum(x => x);
            var chartData = new List<EntriesStatisticChartData>();
            int counter = 0;

            foreach (var item in grouppedEntryModels)
            {
                var category = await _categoriesService.GetCategoryAsync(item.CategoryId);

                if (!category.Result)
                    continue;

                var selectedColor = _colorPallete[counter++];

                chartData.Add(new EntriesStatisticChartData()
                {
                    CategoryName = category.Value?.Name ?? string.Empty,
                    Sum = (double)item.Sum,
                    ColorA = selectedColor.A,
                    ColorR = selectedColor.R,
                    ColorG = selectedColor.G,
                    ColorB = selectedColor.B
                });
            }

            result.ChartData = chartData;

            return Ok(new ServiceResponse<EntriesStatisticDto>(result));
        }

        [HttpGet("{billingPeriodId}", Name = "GetById")]
        public ActionResult<ServiceResponse<BillingPeriodReadDto>> GetById(int billingPeriodId)
        {
            var result = _billingPeriodRepository
                .Get(
                    x => x.Id == billingPeriodId,
                    x => x.Register);

            if (result is null)
            {
                return NotFound(new ServiceResponse(true));
            }

            var billingPeriodResponse = _mapper.Map<BillingPeriodReadDto>(result);

            return Ok(new ServiceResponse<BillingPeriodReadDto>(billingPeriodResponse));
        }

        [HttpPost]
        public async Task<ActionResult<ServiceResponse>> Create(int registerId, BillingPeriodCreateDto billingPeriodCreateDto)
        {
            var response = CheckIsUserTheOwnerOfRegister(registerId);

            if (!response.Item1.Result)
            {
                return StatusCode(response.Item2.StatusCode, response.Item1);
            }

            var billingPeriod = _mapper.Map<BillingPeriod>(billingPeriodCreateDto);
            billingPeriod.RegisterId = registerId;

            _billingPeriodRepository.Add(billingPeriod);
            await _billingPeriodRepository.SaveChangesAsync();

            var billingResponseDto = _mapper.Map<BillingPeriodReadDto>(billingPeriod);

            return CreatedAtRoute(
                nameof(GetById),
                new
                {
                    registerId = registerId,
                    billingPeriodId = billingResponseDto.Id,
                },
                new ServiceResponse<BillingPeriodReadDto>(billingResponseDto));
        }

        [HttpPut("{billingPeriodId}")]
        public async Task<ActionResult<ServiceResponse>> Update(int registerId, int billingPeriodId, BillingPeriodUpdateDto billingPeriodUpdateDto)
        {
            var response = CheckIsUserTheOwnerOfRegister(registerId);

            if (!response.Item1.Result)
            {
                return StatusCode(response.Item2.StatusCode, response.Item1);
            }

            var billingPeriodModel = _billingPeriodRepository.Get(x => x.Id == billingPeriodId);

            if (billingPeriodModel is null)
            {
                return NotFound(
                    new ServiceResponse(
                        new List<string>()
                        {
                            "Billing period not found"
                        }));
            }

            billingPeriodModel.Name = billingPeriodUpdateDto.Name;
            billingPeriodModel.IsOpen = billingPeriodUpdateDto.IsOpen;

            _billingPeriodRepository.Update(billingPeriodModel);
            await _billingPeriodRepository.SaveChangesAsync();

            return Ok(new ServiceResponse(true));
        }

        [HttpDelete("{billingPeriodId}")]
        public async Task<ActionResult<ServiceResponse>> Delete(int registerId, int billingPeriodId)
        {
            var response = CheckIsUserTheOwnerOfRegister(registerId);

            if (!response.Item1.Result)
            {
                return StatusCode(response.Item2.StatusCode, response.Item1);
            }

            var billingPeriodModel = _billingPeriodRepository.Get(x => x.Id == billingPeriodId);

            if (billingPeriodModel is null)
            {
                return NotFound(
                    new ServiceResponse(
                        new List<string>()
                        {
                            "Billing not found"
                        }));
            }

            _billingPeriodRepository.Remove(billingPeriodModel);
            await _billingPeriodRepository.SaveChangesAsync();

            return Ok(new ServiceResponse(true));
        }

        [HttpPut("{billingPeriodId}/Open")]
        public async Task<ActionResult<ServiceResponse>> Open(int registerId, int billingPeriodId)
        {
            var response = CheckIsUserTheOwnerOfRegister(registerId);

            if (!response.Item1.Result)
            {
                return StatusCode(response.Item2.StatusCode, response.Item1);
            }

            var billingPeriodModel = _billingPeriodRepository.Get(x => x.Id == billingPeriodId);

            if (billingPeriodModel is null)
                return NotFound(
                    new ServiceResponse(
                        new List<string>()
                        {
                            "Billing period is not found"
                        }));

            if (billingPeriodModel.IsOpen)
                return BadRequest(
                    new ServiceResponse(
                        new List<string>()
                        {
                            "Billing period is opened already"
                        }));

            billingPeriodModel.IsOpen = true;

            _billingPeriodRepository.Update(billingPeriodModel);
            await _billingPeriodRepository.SaveChangesAsync();

            return Ok(new ServiceResponse(true));
        }

        [HttpPut("{billingPeriodId}/Close")]
        public async Task<ActionResult<ServiceResponse>> Close(int registerId, int billingPeriodId)
        {
            var response = CheckIsUserTheOwnerOfRegister(registerId);

            if (!response.Item1.Result)
            {
                return StatusCode(response.Item2.StatusCode, response.Item1);
            }

            var billingPeriodModel = _billingPeriodRepository.Get(x => x.Id == billingPeriodId);

            if (billingPeriodModel is null)
                return NotFound(
                    new ServiceResponse(
                        new List<string>()
                        {
                            "Billing period is not found"
                        }));

            if (!billingPeriodModel.IsOpen)
                return BadRequest(
                    new ServiceResponse(
                        new List<string>()
                        {
                            "Billing period is opened already"
                        }));

            billingPeriodModel.IsOpen = false;

            _billingPeriodRepository.Update(billingPeriodModel);
            await _billingPeriodRepository.SaveChangesAsync();

            return Ok(new ServiceResponse(true));
        }

        private Tuple<ServiceResponse, StatusCodeResult> CheckIsUserTheOwnerOfRegister(int registerId)
        {
            var userId = GetUserId();

            if (userId is null)
            {
                return new Tuple<ServiceResponse, StatusCodeResult>(
                    new ServiceResponse(
                        new List<string>()
                        {
                            "Missing UserId"
                        }),
                    new StatusCodeResult(404));
            }

            var register = _registerRepository.Get(x => x.Id == registerId);

            if (register is null)
            {
                return new Tuple<ServiceResponse, StatusCodeResult>(
                    new ServiceResponse(
                        new List<string>()
                        {
                            "Register not found"
                        }),
                    new StatusCodeResult(404));
            }

            if (register.CreatorId != userId)
            {
                return new Tuple<ServiceResponse, StatusCodeResult>(
                    new ServiceResponse(
                        new List<string>()
                        {
                            "User is not the owner of the register"
                        }),
                    new StatusCodeResult(400));
            }

            return new Tuple<ServiceResponse, StatusCodeResult>(
                new ServiceResponse(true),
                new StatusCodeResult(200));
        }
    }
}
