using AutoMapper;
using Domain.Controller;
using Domain.Dtos.AccountingService;
using Domain.Model;
using Domain.Services;
using HomeAccountant.AccountingService.Data;
using HomeAccountant.AccountingService.Models;
using HomeAccountant.AccountingService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Reflection.Metadata.Ecma335;

namespace HomeAccountant.AccountingService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RegisterController : ServiceControllerBase
    {
        private readonly IRepository<ApplicationDbContext, Register> _registerRepository;
        private readonly IMapper _mapper;
        private readonly ICategoriesService _categoriesService;
        private readonly IRepository<ApplicationDbContext, Entry> _entryRepository;
        private readonly IRepository<ApplicationDbContext, RegisterSharing> _sharedRegistersRepository;
        private readonly ILogger<RegisterController> _logger;

        public RegisterController(IRepository<ApplicationDbContext, Register> repository,
            IMapper mapper,
            ICategoriesService categoriesService,
            IRepository<ApplicationDbContext, Entry> entryRepository,
            IRepository<ApplicationDbContext, RegisterSharing> sharedRegistersRepository,
            ILogger<RegisterController> logger)
        {
            _registerRepository = repository;
            _mapper = mapper;
            _categoriesService = categoriesService;
            _entryRepository = entryRepository;
            _sharedRegistersRepository = sharedRegistersRepository;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<ServiceResponse<RegisterReadDto>>> Add(RegisterCreateDto registerCreateDto)
        {
            if (!ModelState.IsValid || UserId is null)
                return BadRequest(new ServiceResponse("Invalid payload"));

            var registerModel = _mapper.Map<Register>(registerCreateDto);
            registerModel.CreatorId = UserId;
            _registerRepository.Add(registerModel);
            await _registerRepository.SaveChangesAsync();

            var response = _mapper.Map<RegisterReadDto>(registerModel);

            return CreatedAtRoute(
                nameof(GetRegisterById),
                new
                {
                    id = response.Id
                },
                new ServiceResponse<RegisterReadDto>(response));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ServiceResponse>> Delete(int id)
        {
            var registerModel = _registerRepository.Get(x => x.Id == id);

            if (registerModel is null)
                return NotFound(new ServiceResponse("Register not found"));

            _registerRepository.Remove(registerModel);
            await _registerRepository.SaveChangesAsync();

            return Ok(new ServiceResponse(true));
        }

        [HttpPut]
        public async Task<ActionResult<ServiceResponse>> Update(RegisterUpdateDto registerUpdateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ServiceResponse("Invalid payload"));

            if (UserId is null)
                return BadRequest(new ServiceResponse("Invalid payload"));

            var registerModel = _mapper.Map<Register>(registerUpdateDto);
            registerModel.CreatorId = UserId;

            EntityEntry<Register>? entity = _registerRepository.Update(registerModel);

            if (entity is not null)
                entity.Property(nameof(Register.CreatedDate)).IsModified = false;

            await _registerRepository.SaveChangesAsync();

            return Ok(new ServiceResponse(true));
        }

        [HttpDelete("Entries/{categoryId}")]
        public async Task<ActionResult<ServiceResponse>> DeleteEntriesByCategoryId(int categoryId)
        {
            if (UserId is null)
                return BadRequest(new ServiceResponse("Invalid payload"));

            var entries = _entryRepository.GetAll(x => x.CategoryId == categoryId);

            if (!entries.Any())
                return NotFound(new ServiceResponse("Entries not found"));


            _entryRepository.RemoveMany(entries);
            await _entryRepository.SaveChangesAsync();

            return Ok(new ServiceResponse(true));
        }

        [HttpGet("{id}", Name = "GetRegisterById")]
        public ActionResult<ServiceResponse<RegisterReadDto>> GetRegisterById(int id)
        {
            var registerModel = _registerRepository.Get(x => x.Id == id, x => x.Sharings!);

            if (registerModel is null)
                return NotFound(new ServiceResponse("Register not found"));

            if (UserId is null)
                return BadRequest(new ServiceResponse("Missing UserId"));

            if (registerModel.CreatorId != UserId 
                && !(registerModel.Sharings?.Select(x => x.OwnerId).Contains(UserId) ?? false))
                return Unauthorized(new ServiceResponse("You don't have access to this register"));

            var registerResponse = _mapper.Map<RegisterReadDto>(registerModel);

            return Ok(new ServiceResponse<RegisterReadDto>(registerResponse));
        }

        [HttpGet("SharedRegisters")]
        public ActionResult<ServiceResponse<IEnumerable<RegisterReadDto>?>> GetSharedRegisters()
        {
            try
            {
                if (UserId is null)
                    return BadRequest(
                        new ServiceResponse<IEnumerable<RegisterReadDto>?>(
                            new string[] { "Invalid payload" }));

                var sharedRegisters = _sharedRegistersRepository.GetAll(x => x.OwnerId == UserId, x => x.Register!);

                if (!sharedRegisters.Any())
                    return Ok(
                        new ServiceResponse<IEnumerable<RegisterReadDto>?>(
                            Array.Empty<RegisterReadDto>()));

                var test = sharedRegisters
                        .Select(x => x.Register)
                        .Select(_mapper.Map<RegisterReadDto>);

                return Ok(
                    new ServiceResponse<IEnumerable<RegisterReadDto>?>(
                        sharedRegisters
                        .Select(x => x.Register)
                        .Select(_mapper.Map<RegisterReadDto>)));
            }
            catch (Exception ex)
            {
                _logger.LogError($"--> {ex.Message}");

                return BadRequest(
                    new ServiceResponse<IEnumerable<RegisterReadDto>?>(
                        new string[] { "Reading shared register failed" }));
            }
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<ServiceResponse>> ShareRegister([FromBody] ShareRegisterCreateDto shareRegisterCreateDto)
        {
            try
            {
                if (UserId is null|| string.IsNullOrWhiteSpace(shareRegisterCreateDto.UserId))
                    return BadRequest(new ServiceResponse(new string[] { "Invalid payload" }));

                var register = _registerRepository.Get(x => x.Id == shareRegisterCreateDto.RegisterId);

                if (register is null)
                    return NotFound(new ServiceResponse(new string[] { "Unable to find specified register" }));

                if (register.CreatorId != UserId)
                    return Unauthorized(new ServiceResponse("You cannot share a register that is not yours"));

                _sharedRegistersRepository.Add(new RegisterSharing()
                {
                    OwnerId = shareRegisterCreateDto.UserId,
                    RegisterId = register.Id
                });

                await _sharedRegistersRepository.SaveChangesAsync();

                return Ok(new ServiceResponse(true));
            }
            catch (Exception ex)
            {
                _logger.LogError($"--> {ex.Message}");
                return BadRequest(new ServiceResponse(new string[] { "Sharing register failed" }));
            }
        }

        [HttpGet]
        public ActionResult<ServiceResponse<IEnumerable<RegisterReadDto>>> Get()
        {
            if (UserId is null)
                return BadRequest(new ServiceResponse("Missing UserId"));

            var registers = _registerRepository.GetAll(x => x.CreatorId == UserId).OrderByDescending(x => x.CreatedDate);

            return Ok(
                new ServiceResponse<IEnumerable<RegisterReadDto>>(
                    _mapper.Map<IEnumerable<RegisterReadDto>>(registers)));
        }
    }
}
