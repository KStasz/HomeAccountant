using AutoMapper;
using Domain.Controller;
using Domain.Dtos.CategoryService;
using Domain.Model;
using Domain.Services;
using HomeAccountant.CategoriesService.Data;
using HomeAccountant.CategoriesService.Model;
using HomeAccountant.CategoriesService.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeAccountant.CategoriesService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CategoriesController : ServiceControllerBase
    {
        private readonly IRepository<ApplicationDbContext, CategoryModel> _categoriesService;
        private readonly IMapper _mapper;
        private readonly IAccountingService _registerService;

        public CategoriesController(IRepository<ApplicationDbContext, CategoryModel> categoriesService,
            IMapper mapper,
            IAccountingService registerService)
        {
            _categoriesService = categoriesService;
            _mapper = mapper;
            _registerService = registerService;
        }

        [HttpGet("{id:int}", Name = "GetById")]
        public ActionResult<ServiceResponse<CategoryReadDto>> GetById(int id)
        {
            CategoryModel? categoryModel = _categoriesService.Get(x => x.Id == id);

            if (categoryModel is null)
            {
                return NotFound(new ServiceResponse("Unable to find specific category"));
            }

            var categoryRead = _mapper.Map<CategoryReadDto>(categoryModel);

            return Ok(new ServiceResponse<CategoryReadDto>(categoryRead));
        }

        [HttpGet]
        public ActionResult<ServiceResponse<IEnumerable<CategoryReadDto>>> GetAll()
        {
            if (UserId is null)
                return BadRequest(new ServiceResponse("Invalid payload"));

            var categories = _categoriesService.GetAll(x => x.CreatedBy == UserId);

            return Ok(new ServiceResponse<IEnumerable<CategoryReadDto>>(
                _mapper.Map<IEnumerable<CategoryReadDto>>(categories)));
        }

        [HttpPut]
        public async Task<ActionResult<ServiceResponse>> Update([FromBody] CategoryUpdateDto categoryUpdateDto)
        {
            var categoryModel = _categoriesService.Get(x => x.Id == categoryUpdateDto.Id);

            if (categoryModel is null)
                return NotFound(new ServiceResponse("Unable to find specific category"));

            categoryModel.Name = categoryUpdateDto.Name;

            _categoriesService.Update(categoryModel);
            await _categoriesService.SaveChangesAsync();

            return Ok(new ServiceResponse(true));
        }

        [HttpPost]
        public async Task<ActionResult<ServiceResponse<CategoryReadDto>>> Add([FromBody] CategoryCreateDto categoryCreateDto)
        {
            var categoryModel = _mapper.Map<CategoryModel>(categoryCreateDto);
            
            if (UserId is null)
            {
                return BadRequest(new ServiceResponse("Invalid payload"));
            }

            categoryModel.CreatedBy = UserId;

            _categoriesService.Add(categoryModel);
            await _categoriesService.SaveChangesAsync();

            var categoryRead = _mapper.Map<CategoryReadDto>(categoryModel);

            return CreatedAtRoute(
                nameof(GetById),
                new { id = categoryModel.Id },
                new ServiceResponse<CategoryReadDto>(categoryRead));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ServiceResponse>> Delete(int id)
        {
            var categoryModel = _categoriesService.Get(x => x.Id == id);

            if (categoryModel is null)
                return NotFound(new ServiceResponse("Unable to find specific category"));

            await _registerService.DeleteEntriesByCategoryId(categoryModel.Id);

            _categoriesService.Remove(categoryModel);
            await _categoriesService.SaveChangesAsync();

            return Ok(new ServiceResponse(true));
        }
    }
}
