﻿using AutoMapper;
using Domain.Dtos;
using Domain.Dtos.AccountingService;
using HomeAccountant.AccountingService.Models;
using HomeAccountant.AccountingService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;
using System.Reflection.Metadata.Ecma335;

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
        public async Task<ActionResult<ServiceResponse<RegisterReadDto>>> Add(RegisterCreateDto registerCreateDto)
        {
            var userId = GetUserId();

            if (!ModelState.IsValid || userId is null)
                return BadRequest(new ServiceResponse("Invalid payload"));

            var registerModel = _mapper.Map<Register>(registerCreateDto);
            registerModel.CreatorId = userId;
            _repository.Add(registerModel);
            await _repository.SaveChangesAsync();

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
            var registerModel = _repository.Get(x => x.Id == id);

            if (registerModel is null)
                return NotFound(new ServiceResponse("Register not found"));

            _repository.Remove(registerModel);
            await _repository.SaveChangesAsync();

            return Ok(new ServiceResponse(true));
        }

        [HttpPut]
        public async Task<ActionResult<ServiceResponse>> Update(RegisterUpdateDto registerUpdateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ServiceResponse("Invalid payload"));

            var userId = GetUserId();

            if (userId is null)
                return BadRequest(new ServiceResponse("Invalid payload"));

            var registerModel = _mapper.Map<Register>(registerUpdateDto);
            registerModel.CreatorId = userId;

            EntityEntry entity = _repository.Update(registerModel);
            entity.Property(nameof(Register.CreatedDate)).IsModified = false;
            await _repository.SaveChangesAsync();

            return Ok(new ServiceResponse(true));
        }

        [HttpDelete("Entries/{categoryId}")]
        public async Task<ActionResult<ServiceResponse>> DeleteEntriesByCategoryId(int categoryId)
        {
            var userId = GetUserId();

            if (userId is null)
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
            var registerModel = _repository.Get(x => x.Id == id);
            var userId = GetUserId();

            if (registerModel is null)
                return NotFound(new ServiceResponse("Register not found"));

            if (userId is null)
                return BadRequest(new ServiceResponse("Missing UserId"));

            if (registerModel.CreatorId != userId)
                return Unauthorized(new ServiceResponse("You don't have access to this resource"));

            var registerResponse = _mapper.Map<RegisterReadDto>(registerModel);

            return Ok(new ServiceResponse<RegisterReadDto>(registerResponse));
        }

        [HttpGet]
        public ActionResult<ServiceResponse<IEnumerable<RegisterReadDto>>> Get()
        {
            var userId = GetUserId();

            if (userId is null)
                return BadRequest(new ServiceResponse("Missing UserId"));

            var registers = _repository.GetAll(x => x.CreatorId == userId).OrderByDescending(x => x.CreatedDate);

            return Ok(
                new ServiceResponse<IEnumerable<RegisterReadDto>>(
                    _mapper.Map<IEnumerable<RegisterReadDto>>(registers)));
        }
    }
}
