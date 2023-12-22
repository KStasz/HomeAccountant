using AutoMapper;
using Domain.Controller;
using Domain.Dtos.CategoryService;
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
        public IActionResult GetById(int id)
        {
            CategoryModel? categoryModel = _categoriesService.Get(x => x.Id == id);

            if (categoryModel is null)
            {
                return NotFound();
            }

            var categoryRead = _mapper.Map<CategoryReadDto>(categoryModel);

            return Ok(categoryRead);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var userId = GetUserId();
            if (userId is null)
                return BadRequest("Invalid payload");

            var categories = _categoriesService.GetAll(x => x.CreatedBy == userId);

            return Ok(_mapper.Map<IEnumerable<CategoryReadDto>>(categories));
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] CategoryUpdateDto categoryUpdateDto)
        {
            var categoryModel = _categoriesService.Get(x => x.Id == categoryUpdateDto.Id);

            if (categoryModel is null)
                return NotFound();

            categoryModel.Name = categoryUpdateDto.Name;

            _categoriesService.Update(categoryModel);
            await _categoriesService.SaveChangesAsync();

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CategoryCreateDto categoryCreateDto)
        {
            var categoryModel = _mapper.Map<CategoryModel>(categoryCreateDto);

            var userId = GetUserId();

            if (userId is null)
            {
                return BadRequest("Invalid payload");
            }

            categoryModel.CreatedBy = userId;

            _categoriesService.Add(categoryModel);
            await _categoriesService.SaveChangesAsync();

            var categoryRead = _mapper.Map<CategoryReadDto>(categoryModel);

            return CreatedAtRoute(nameof(GetById), new { id = categoryModel.Id }, categoryRead);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var categoryModel = _categoriesService.Get(x => x.Id == id);

            if (categoryModel is null)
                return NotFound();

            await _registerService.DeleteEntriesByCategoryId(categoryModel.Id);

            _categoriesService.Remove(categoryModel);
            await _categoriesService.SaveChangesAsync();

            return Ok();
        }
    }
}
