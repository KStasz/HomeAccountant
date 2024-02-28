using HomeAccountant.Core.Model;
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

        public IAlert? PageAlerts { get; set; }
        public IModalDialog<CategoryModel>? DeleteCategoryDialog { get; set; }
        public IModalDialog<CategoryModel, CategoryModel>? CreateCategoryDialog { get; set; }

        private IEnumerable<CategoryModel>? _categories;
        public IEnumerable<CategoryModel>? Categories
        {
            get => _categories;
            set => SetValue(ref _categories, value);
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

            await CreateCategoryDialog.InitializeDialogAsync(new CategoryModel());

            var result = await CreateCategoryDialog.ShowModalAsync(CancellationToken);

            if (result is null)
                return;

            var response = await _categoriesService.CreateCategoryAsync(result, CancellationToken);

            if (!response.Result)
            {
                if (PageAlerts is null)
                    return;

                await PageAlerts!.ShowAlertAsync(
                    string.Join(
                        Environment.NewLine,
                        response.Errors ?? Array.Empty<string>()),
                    AlertType.Danger,
                    CancellationToken);

                return;
            }

            await ReadCategoriesAsync(CancellationToken);
        }

        public async Task DeleteCategoryAsync(CategoryModel categoryReadDto)
        {
            if (DeleteCategoryDialog is null)
                return;

            await DeleteCategoryDialog.InitializeDialogAsync(categoryReadDto);
            var result = await DeleteCategoryDialog.ShowModalAsync(CancellationToken);

            if (result == ModalResult.Cancel)
                return;

            var response = await _categoriesService.DeleteCategoryAsync(categoryReadDto.Id, CancellationToken);

            if (!response.Result)
            {
                if (PageAlerts is null)
                    return;

                await PageAlerts.ShowAlertAsync(
                    string.Join(
                        Environment.NewLine,
                        response.Errors ?? Array.Empty<string>()),
                    AlertType.Danger,
                    CancellationToken);
                
                return;
            }

            await ReadCategoriesAsync(CancellationToken);
        }

        private async Task ReadCategoriesAsync(CancellationToken cancellationToken = default)
        {
            var response = await _categoriesService.GetCategoriesAsync(cancellationToken);

            if (!response.Result)
            {
                if (PageAlerts is null)
                    return;

                await PageAlerts.ShowAlertAsync(
                    string.Join(
                        Environment.NewLine,
                        response.Errors ?? Array.Empty<string>()),
                    AlertType.Danger,
                    cancellationToken);

                Categories = new List<CategoryModel>();
                
                return;
            }

            Categories = response.Value!;
        }
    }
}
