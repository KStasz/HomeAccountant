using HomeAccountant.Core.Model;
using HomeAccountant.Core.Services;
using Moq;
using Xunit;
using HomeAccountant.CoreTests.Comparers;
using Assert = Xunit.Assert;

namespace HomeAccountant.Core.ViewModels.Tests
{
    public class CategoriesViewModelTests
    {
        private Mock<ICategoriesService> _categoriesServiceMock;
        private CategoriesViewModel _viewModel;
        private Mock<IModalDialog<CategoryModel, CategoryModel>> _createCategoryDialogMock;
        private Mock<IAlert> _alertMock;
        private Mock<IModalDialog<CategoryModel>> _deleteCategoryDialogMock;

        public CategoriesViewModelTests()
        {
            _categoriesServiceMock = new Mock<ICategoriesService>();
            _createCategoryDialogMock = new Mock<IModalDialog<CategoryModel, CategoryModel>>();
            _alertMock = new Mock<IAlert>();
            _deleteCategoryDialogMock = new Mock<IModalDialog<CategoryModel>>();
            _viewModel = new CategoriesViewModel(_categoriesServiceMock.Object);

            _viewModel.CreateCategoryDialog = _createCategoryDialogMock.Object;
            _viewModel.DeleteCategoryDialog = _deleteCategoryDialogMock.Object;
            _viewModel.PageAlerts = _alertMock.Object;
        }

        [Fact()]
        public async Task PageInitializedAsync_ShouldAssignCategoriesToProperty()
        {
            var expected = new List<CategoryModel>()
            {
                new CategoryModel()
                {
                    Id = 1,
                    Name = "Test",
                }
            }.AsEnumerable();

            _categoriesServiceMock.Setup(x => x.GetCategoriesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ServiceResponse<IEnumerable<CategoryModel>?>(expected));

            await _viewModel.PageInitializedAsync();

            Assert.NotNull(_viewModel.Categories);
            Assert.Equal(expected, _viewModel.Categories);
            Assert.False(_viewModel.IsBusy);
        }

        [Fact()]
        public async Task PageInitializedAsync_ShouldShowErrorIfReadingCategoriesReturnsError()
        {
            _categoriesServiceMock.Setup(x => x.GetCategoriesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ServiceResponse<IEnumerable<CategoryModel>?>(false, It.IsAny<string[]>()));

            await _viewModel.PageInitializedAsync();

            Assert.NotNull(_viewModel.Categories);
            Assert.False(_viewModel.IsBusy);
            _alertMock.Verify(
                x => x.ShowAlertAsync(
                    It.IsAny<string>(),
                    It.IsAny<AlertType>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact()]
        public async Task CreateCategoryAsync_ShouldSkipMethodIfCreateCategoryDialogIsNull()
        {
            await _viewModel.CreateCategoryAsync();

            Assert.Null(_viewModel.Categories);
        }

        [Fact()]
        public async Task CreateCategoryAsync_ShouldShowErrorIfCreatingCategoryFailed()
        {
            _createCategoryDialogMock.Setup(
                    x => x.ShowModalAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    new CategoryModel()
                    {
                        Id = 1,
                        Name = "Test",
                    });

            _categoriesServiceMock.Setup(
                x => x.CreateCategoryAsync(
                    It.IsAny<CategoryModel>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    new ServiceResponse<CategoryModel?>(
                        false,
                        It.IsAny<string[]>()));

            await _viewModel.CreateCategoryAsync();

            _alertMock.Verify(
                x => x.ShowAlertAsync(
                    It.IsAny<string>(),
                    It.IsAny<AlertType>(),
                    It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact()]
        public async Task CreateCategoryAsync_ShouldCreateCategoryIntoCategoriesCollection()
        {
            var expected = new List<CategoryModel>()
            {
                new CategoryModel()
                {
                    Name = "Test category"
                }
            }.AsEnumerable();

            var createdCategory = new CategoryModel()
            {
                Name = "Test category"
            };

            var serviceResponse = new ServiceResponse<IEnumerable<CategoryModel>?>(
                new List<CategoryModel>()
                {
                    createdCategory
                });

            _categoriesServiceMock.Setup(x => x.CreateCategoryAsync(
                    It.IsAny<CategoryModel>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ServiceResponse<CategoryModel?>(createdCategory));

            _categoriesServiceMock.Setup(x => x.GetCategoriesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(serviceResponse);

            _createCategoryDialogMock.Setup(x => x.ShowModalAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdCategory);

            await _viewModel.CreateCategoryAsync();

            Assert.NotNull(_viewModel.Categories);
            Assert.Equal(expected, _viewModel.Categories, new CategoryModelCollectionComparer());
        }

        [Fact()]
        public async Task DeleteCategoryAsync_ShouldDeleteSelectedCategory()
        {
            var actual = new List<CategoryModel>()
            {
                new CategoryModel()
                {
                    Id = 1,
                    Name = "Test1"
                },
                new CategoryModel()
                {
                    Id = 2,
                    Name = "Test2"
                }
            }.AsEnumerable();

            var categoryToDelete = actual.Last();

            var expected = new List<CategoryModel>()
            {
                new CategoryModel()
                {
                    Id = 1,
                    Name = "Test1"
                }
            }.AsEnumerable();

            _deleteCategoryDialogMock.Setup(x => x.ShowModalAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(ModalResult.Success);

            _categoriesServiceMock.Setup(x => x.DeleteCategoryAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ServiceResponse(true));

            _categoriesServiceMock.Setup(x => x.GetCategoriesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ServiceResponse<IEnumerable<CategoryModel>?>(actual));

            await _viewModel.PageInitializedAsync();

            _categoriesServiceMock.Setup(x => x.GetCategoriesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ServiceResponse<IEnumerable<CategoryModel>?>(expected));

            await _viewModel.DeleteCategoryAsync(categoryToDelete);

            Assert.NotNull(_viewModel.Categories);
            Assert.Equal(expected, _viewModel.Categories);
            Assert.False(_viewModel.IsBusy);
            _categoriesServiceMock.Verify(
                x => x.DeleteCategoryAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact()]
        public async Task DeleteCategoryAsync_ShouldShowErrorIfDeletingFailed()
        {
            var actual = new List<CategoryModel>()
            {
                new CategoryModel()
                {
                    Id = 1,
                    Name = "Test1"
                },
                new CategoryModel()
                {
                    Id = 2,
                    Name = "Test2"
                }
            }.AsEnumerable();

            _categoriesServiceMock.Setup(x => x.GetCategoriesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ServiceResponse<IEnumerable<CategoryModel>?>(actual));

            await _viewModel.PageInitializedAsync();

            _deleteCategoryDialogMock.Setup(x => x.ShowModalAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(ModalResult.Success);

            _categoriesServiceMock.Setup(x => x.DeleteCategoryAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ServiceResponse(false, [It.IsAny<string>()]));

            await _viewModel.DeleteCategoryAsync(actual.Last());

            _alertMock.Verify(
                x => x.ShowAlertAsync(
                    It.IsAny<string>(),
                    It.IsAny<AlertType>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
            Assert.False(_viewModel.IsBusy);
        }
    }
}