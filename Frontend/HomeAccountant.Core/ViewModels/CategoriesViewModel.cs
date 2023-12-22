using HomeAccountant.Core.DTOs.Category;
using HomeAccountant.Core.Services;

namespace HomeAccountant.Core.ViewModels
{
    public class CategoriesViewModel : MvvmViewModel
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

        public override async Task PageInitializedAsync()
        {
            IsBusy = true;
            await ReadCategoriesAsync(CancellationToken);
            IsBusy = false;
        }

        public async Task CreateCategoryAsync()
        {
            if (CreateCategoryDialog is null)
                return;

            await CreateCategoryDialog.InitializeDialogAsync(new CategoryCreateDto());

            var result = await CreateCategoryDialog.ShowModalAsync(CancellationToken);

            if (result is null)
                return;

            await _categoriesService.CreateCategoryAsync(result, CancellationToken);

            await ReadCategoriesAsync(CancellationToken);
        }

        public async Task DeleteCategoryAsync(CategoryReadDto categoryReadDto)
        {
            if (DeleteCategoryDialog is null)
                return;

            await DeleteCategoryDialog.InitializeDialogAsync(categoryReadDto);
            var result = await DeleteCategoryDialog.ShowModalAsync(CancellationToken);

            if (result == ModalResult.Cancel)
                return;

            await _categoriesService.DeleteCategoryAsync(categoryReadDto.Id, CancellationToken);

            await ReadCategoriesAsync(CancellationToken);
        }

        private async Task ReadCategoriesAsync(CancellationToken cancellationToken = default)
        {
            var response = await _categoriesService.GetCategoriesAsync(cancellationToken);

            if (!response.Result)
                Categories = new List<CategoryReadDto>();

            Categories = response.Value;
        }
    }
}
