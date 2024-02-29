using AutoMapper;
using Domain.Dtos.CategoryService;
using Domain.Model;
using Domain.Services;
using HomeAccountant.CategoriesService.Data;
using HomeAccountant.CategoriesService.Model;
using HomeAccountant.CategoriesService.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using Xunit;
using Assert = Xunit.Assert;

namespace HomeAccountant.CategoriesService.Controllers.Tests
{
    public class CategoriesControllerTests
    {
        private readonly Mock<IRepository<ApplicationDbContext, CategoryModel>> _dbContext;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IAccountingService> _accountingServiceMock;
        private readonly CategoriesController _categoriesController;

        public CategoriesControllerTests()
        {
            _dbContext = new Mock<IRepository<ApplicationDbContext, CategoryModel>>();
            _mapperMock = new Mock<IMapper>();
            _accountingServiceMock = new Mock<IAccountingService>();

            _categoriesController = new CategoriesController(
                _dbContext.Object,
                _mapperMock.Object,
                _accountingServiceMock.Object);
        }

        [Fact()]
        public void GetById_ShouldReturnCategoryModelBasingOnId()
        {
            CategoryModel? expected = new CategoryModel()
            {
                Id = It.IsAny<int>(),
                Name = It.IsAny<string>(),
                IsActive = It.IsAny<bool>(),
                CreatedBy = It.IsAny<string>(),
                CreationDate = It.IsAny<DateTime>()
            };
            CategoryReadDto expectedDto = new CategoryReadDto()
            {
                Id = It.IsAny<int>(),
                Name = It.IsAny<string>(),
            };

            _dbContext.Setup(
                x => x.Get(It.IsAny<Func<CategoryModel, bool>>()))
                .Returns(expected);
            _mapperMock.Setup(
                x => x.Map<CategoryReadDto>(It.IsAny<CategoryModel>()))
                .Returns(expectedDto);

            var result = _categoriesController.GetById(It.IsAny<int>());

            Assert.IsType<OkObjectResult>(result.Result);
            Assert.IsType<ServiceResponse<CategoryReadDto>>(((OkObjectResult)result.Result).Value);
            Assert.NotNull(((ServiceResponse<CategoryReadDto>?)((OkObjectResult)result.Result).Value)?.Value);
        }

        [Fact()]
        public void GetAll_ShouldReturnAllCategoriesAssignedToUser()
        {
            MockHttpContext("User");
            var expectedCollection = new List<CategoryModel>()
            {
                new CategoryModel()
                {
                    Id = It.IsAny<int>(),
                    Name = It.IsAny<string>(),
                    IsActive = It.IsAny<bool>(),
                    CreationDate = It.IsAny<DateTime>(),
                    CreatedBy = It.IsAny<string>()
                },
                new CategoryModel()
                {
                    Id = It.IsAny<int>(),
                    Name = It.IsAny<string>(),
                    IsActive = It.IsAny<bool>(),
                    CreationDate = It.IsAny<DateTime>(),
                    CreatedBy = It.IsAny<string>()
                }
            }.AsEnumerable();

            var expectedMappedCollection = new List<CategoryReadDto>()
            {
                new CategoryReadDto()
                {
                    Id = It.IsAny<int>(),
                    Name = It.IsAny<string>(),
                },
                new CategoryReadDto()
                {
                    Id = It.IsAny<int>(),
                    Name = It.IsAny<string>(),
                }
            }.AsEnumerable();

            _dbContext.Setup(
                x => x.GetAll(It.IsAny<Func<CategoryModel, bool>>()))
                .Returns(expectedCollection);

            _mapperMock.Setup(
                x => x.Map<IEnumerable<CategoryReadDto>>(expectedCollection))
                .Returns(expectedMappedCollection);

            var response = _categoriesController.GetAll();

            Assert.IsType<OkObjectResult>(response.Result);
            Assert.IsType<ServiceResponse<IEnumerable<CategoryReadDto>>>(((OkObjectResult)response.Result).Value);
            Assert.NotNull(((ServiceResponse<IEnumerable<CategoryReadDto>>?)((OkObjectResult)response.Result).Value)?.Value);
        }

        [Fact()]
        public async Task Update_Test()
        {
            var categoryModel = new CategoryModel()
            {
                Id = It.IsAny<int>(),
                Name = It.IsAny<string>(),
                IsActive = It.IsAny<bool>(),
                CreatedBy = It.IsAny<string>(),
                CreationDate = It.IsAny<DateTime>()
            };

            var categoryUpdateModel = new CategoryUpdateDto()
            {
                Id = It.IsAny<int>(),
                Name = It.IsAny<string>()
            };

            _dbContext.Setup(
                x => x.Get(It.IsAny<Func<CategoryModel, bool>>()))
                .Returns(categoryModel);


            var response = await _categoriesController.Update(categoryUpdateModel);

            Assert.IsType<OkObjectResult>(response.Result);
            Assert.IsType<ServiceResponse>(((OkObjectResult)response.Result).Value);
            Assert.True(((ServiceResponse?)((OkObjectResult)response.Result).Value)?.Result);
        }

        [Fact()]
        public async Task Add_Test()
        {
            MockHttpContext("User");

            var categoryModel = new CategoryModel()
            {
                Id = It.IsAny<int>(),
                Name = It.IsAny<string>(),
                CreationDate = It.IsAny<DateTime>(),
                CreatedBy = It.IsAny<string>(),
                IsActive = It.IsAny<bool>(),
            };

            var categoryReadDto = new CategoryReadDto()
            {
                Id = It.IsAny<int>(),
                Name = It.IsAny<string>()
            };

            _mapperMock.Setup(
                x => x.Map<CategoryModel>(It.IsAny<CategoryCreateDto>()))
                .Returns(categoryModel);
            _mapperMock.Setup(
                x => x.Map<CategoryReadDto>(It.IsAny<CategoryModel>()))
                .Returns(categoryReadDto);

            var response = await _categoriesController.Add(It.IsAny<CategoryCreateDto>());

            Assert.IsType<CreatedAtRouteResult>(response.Result);
            Assert.IsType<ServiceResponse<CategoryReadDto>>(((CreatedAtRouteResult)response.Result).Value);
            Assert.NotNull(((ServiceResponse<CategoryReadDto>?)((CreatedAtRouteResult)response.Result).Value)?.Value);
        }

        [Fact()]
        public async Task Delete_Test()
        {
            var categoryModel = new CategoryModel()
            {
                Id = It.IsAny<int>(),
                Name = It.IsAny<string>(),
                IsActive = It.IsAny<bool>(),
                CreatedBy = It.IsAny<string>(),
                CreationDate = It.IsAny<DateTime>()
            };

            _dbContext.Setup(x => x.Get(It.IsAny<Func<CategoryModel, bool>>()))
                .Returns(categoryModel);

            var response = await _categoriesController.Delete(It.IsAny<int>());

            Assert.IsType<OkObjectResult>(response.Result);
            Assert.IsType<ServiceResponse>(((OkObjectResult)response.Result).Value);
            Assert.True(((ServiceResponse?)((OkObjectResult)response.Result).Value)?.Result);
        }

        private void MockHttpContext(string userId)
        {
            var claimsMock = new Mock<ClaimsPrincipal>();
            var httpContextMock = new Mock<HttpContext>();
            IEnumerable<Claim> claims = new List<Claim>()
            {
                new Claim("UserId", userId)
            }.AsEnumerable();

            claimsMock.Setup(x => x.Claims)
                .Returns(claims);

            httpContextMock.Setup(
                x => x.User)
                .Returns(claimsMock.Object);

            _categoriesController.ControllerContext = new ControllerContext();
            _categoriesController.ControllerContext.HttpContext = httpContextMock.Object;
        }
    }
}