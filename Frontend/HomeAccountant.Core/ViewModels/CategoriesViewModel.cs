using HomeAccountant.Core.DTOs.Category;
using HomeAccountant.Core.Services;

namespace HomeAccountant.Core.ViewModels
{
    public class CategoriesViewModel : BaseViewModel
    {
        private readonly ICategoriesService _categoriesService;

        public CategoriesViewModel(ICategoriesService categoriesService)
        {
            _categoriesService = categoriesService;
        }

        public IModalDialog<CategoryReadDto>? DeleteCategoryDialog { get; set; }
        public IModalDialog<CategoryCreateDto, CategoryCreateDto>? CreateCategoryDialog { get; set; }

        private IEnumerable<CategoryReadDto>? _categories;
        public IEnumerable<CategoryReadDto>? Categories
        {
            get
            {
                return _categories;
            }
            set
            {
                _categories = value;
                NotifyPropertyChanged();
            }
        }

        public async Task InitializeAsync()
        {
            await ReadCategoriesAsync();
        }

        private async Task ReadCategoriesAsync()
        {
            var response = await _categoriesService.GetCategoriesAsync();

            if (!response.IsSucceed)
                Categories = new List<CategoryReadDto>();

            Categories = response.Result;
        }

        public async Task CreateCategoryAsync()
        {
            if (CreateCategoryDialog is null)
                return;

            await CreateCategoryDialog.InitializeDialogAsync(new CategoryCreateDto());

            var result = await CreateCategoryDialog.ShowModalAsync();

            if (result is null)
                return;

            await _categoriesService.CreateCategoryAsync(result);

            await ReadCategoriesAsync();
        }

        public async Task DeleteCategoryAsync(CategoryReadDto categoryReadDto)
        {
            if (DeleteCategoryDialog is null)
                return;

            await DeleteCategoryDialog.InitializeDialogAsync(categoryReadDto);
            var result = await DeleteCategoryDialog.ShowModalAsync();

            if (result == ModalResult.Cancel)
                return;

            await _categoriesService.DeleteCategoryAsync(categoryReadDto.Id);

            await ReadCategoriesAsync();
        }
    }
}
