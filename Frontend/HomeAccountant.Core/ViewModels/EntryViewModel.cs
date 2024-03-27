using HomeAccountant.Core.Model;
using HomeAccountant.Core.Services;
using System.Collections.ObjectModel;

namespace HomeAccountant.Core.ViewModels
{
    public class EntryViewModel : BaseViewModel
    {
        private readonly IEntryService _entryService;
        private readonly ICategoriesService _categoriesService;
        private int _billingPeriodId;
        private int _registerId;
        private bool _isPeriodOpen;

        public EntryViewModel(IEntryService entryService,
            ICategoriesService categoriesService)
        {
            _entryService = entryService;
            _categoriesService = categoriesService;

            _entries = new ObservableCollection<EntryModel>();
        }

        public IModalDialog<EntryModel, EntryModel>? EntryCreateDialog { get; set; }
        public IModalDialog<EntryModel>? EntryDeleteDialog { get; set; }

        public Func<Task>? NotifyEntryHasBeenCreated { get; set; }

        private ObservableCollection<EntryModel> _entries;
        public ObservableCollection<EntryModel> Entries
        {
            get => _entries;
            set => SetValue(ref _entries, value);
        }

        private IEnumerable<CategoryModel>? _availableCategories;
        public IEnumerable<CategoryModel>? AvailableCategories
        {
            get => _availableCategories;
            set => SetValue(ref _availableCategories, value);
        }

        private int _currentPage = 1;
        public int CurrentPage
        {
            get => _currentPage;
            set => SetValue(ref _currentPage, value);
        }

        private int _totalPages;
        public int TotalPages
        {
            get => _totalPages;
            set => SetValue(ref _totalPages, value);
        }

        private IEnumerable<int>? _availablePagesCollection;
        public IEnumerable<int>? AvailablePagesCollection
        {
            get => _availablePagesCollection;
            set => SetValue(ref _availablePagesCollection, value);
        }

        public string IsNextPageButtonDisabled
        {
            get
            {
                return CurrentPage >= TotalPages ? "disabled" : string.Empty;
            }
        }

        public string IsPreviousPageButtonDisabled
        {
            get
            {
                return CurrentPage <= 1 ? "disabled" : string.Empty;
            }
        }

        public Dictionary<string, object> IsButtonBlocked
        {
            get
            {
                return _isPeriodOpen ? new Dictionary<string, object>() : new Dictionary<string, object>() { { "disabled", "" } };
            }
        }

        public async Task SetParametersAsync(int billingPeriodId, int registerId, bool isPeriodOpen)
        {
            IsBusy = true;
            _billingPeriodId = billingPeriodId;
            _registerId = registerId;
            _isPeriodOpen = isPeriodOpen;
            CurrentPage = 1;
            TotalPages = 0;

            await ReadEntriesAsync(CancellationToken);
            await ReadCategoriesAsync(CancellationToken);

            IsBusy = false;
        }

        public async Task RefreshDataAsync()
        {
            await ReadEntriesAsync(CancellationToken);
        }

        public void RefreshBillingPeriodState(bool isPeriodOpen)
        {
            _isPeriodOpen = isPeriodOpen;
            NotifyPropertyChangedAsync(nameof(IsButtonBlocked));
        }

        public async Task SetPageAsync(int page)
        {
            IsBusy = true;
            CurrentPage = page;
            await ReadEntriesAsync(CancellationToken);
            IsBusy = false;
        }

        public async Task NextPageAsync()
        {
            if (CurrentPage < TotalPages)
            {
                IsBusy = true;
                CurrentPage++;
                await ReadEntriesAsync(CancellationToken);
                IsBusy = false;
            }
        }

        public async Task PreviousPageAsync()
        {
            if (CurrentPage > 1)
            {
                IsBusy = true;
                CurrentPage--;
                await ReadEntriesAsync(CancellationToken);
                IsBusy = false;
            }
        }

        public async Task CreateEntryAsync()
        {
            if (EntryCreateDialog is null)
            {
                return;
            }

            await EntryCreateDialog.InitializeDialogAsync(new EntryModel());

            var result = await EntryCreateDialog.ShowModalAsync(CancellationToken);

            if (result is null)
            {
                return;
            }

            var creationResult = await _entryService.CreateEntryAsync(_registerId, _billingPeriodId, result, CancellationToken);

            if (NotifyEntryHasBeenCreated is null)
                return;

            await NotifyEntryHasBeenCreated.Invoke();
        }

        public async Task DeleteEntry(EntryModel entryReadDto)
        {
            if (EntryDeleteDialog is null)
                return;

            await EntryDeleteDialog.InitializeDialogAsync(entryReadDto);

            var result = await EntryDeleteDialog.ShowModalAsync(CancellationToken);

            if (result == ModalResult.Cancel)
                return;

            await _entryService.DeleteEntryAsync(_registerId, _billingPeriodId, entryReadDto.Id, CancellationToken);

            if (NotifyEntryHasBeenCreated is null)
                return;

            await NotifyEntryHasBeenCreated.Invoke();
        }

        private async Task ReadCategoriesAsync(CancellationToken cancellationToken)
        {
            var result = await _categoriesService.GetRegisterCategories(_registerId, cancellationToken);

            if (!result.Result)
                return;

            AvailableCategories = result.Value;
        }

        private void CalculateAvailablePages()
        {
            var pages = Enumerable.Range(1, TotalPages)
                .Chunk(5);
            AvailablePagesCollection = pages.FirstOrDefault(x => x.Contains(CurrentPage));
        }

        private async Task ReadEntriesAsync(CancellationToken cancellationToken)
        {
            var result = await _entryService.GetEntriesAsync(_registerId, _billingPeriodId, CurrentPage, cancellationToken: cancellationToken);

            if (!result.Result)
                return;

            Entries = new ObservableCollection<EntryModel>(result.Value?.Result ?? Array.Empty<EntryModel>());
            CurrentPage = result.Value?.CurrentPage ?? 0;
            TotalPages = result.Value?.TotalPages ?? 0;
            CalculateAvailablePages();
        }

    }
}
