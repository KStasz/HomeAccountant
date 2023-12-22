using HomeAccountant.Core.DTOs.BillingPeriod;
using HomeAccountant.Core.DTOs.Category;
using HomeAccountant.Core.DTOs.Entry;
using HomeAccountant.Core.Services;

namespace HomeAccountant.Core.ViewModels
{
    public class EntryViewModel : MvvmViewModel
    {
        private readonly IEntryService _entryService;
        private readonly ICategoriesService _categoriesService;
        private readonly IBillingPeriodService _billingPeriodService;
        private BillingPeriodReadDto? _billingPerdiod;
        private int _registerId;

        public EntryViewModel(IEntryService entryService,
            ICategoriesService categoriesService,
            IBillingPeriodService billingPeriodService)
        {
            _entryService = entryService;
            _categoriesService = categoriesService;
            _billingPeriodService = billingPeriodService;
        }

        public IModalDialog<EntryCreateDto, EntryCreateDto>? EntryCreateDialog { get; set; }
        public IModalDialog<EntryReadDto>? EntryDeleteDialog { get; set; }

        private IEnumerable<EntryReadDto>? _entries;
        public IEnumerable<EntryReadDto>? Entries
        {
            get
            {
                return _entries;
            }
            set
            {
                _entries = value;
                NotifyPropertyChanged();
            }
        }

        public Func<Task>? RefreshChart { get; set; }

        private IEnumerable<CategoryReadDto>? _availableCategories;
        public IEnumerable<CategoryReadDto>? AvailableCategories
        {
            get
            {
                return _availableCategories;
            }
            set
            {
                _availableCategories = value;
                NotifyPropertyChanged();
            }
        }

        private int _currentPage = 1;
        public int CurrentPage
        {
            get
            {
                return _currentPage;
            }
            set
            {
                _currentPage = value;
                NotifyPropertyChanged();
            }
        }

        private int _totalPages;
        public int TotalPages
        {
            get
            {
                return _totalPages;
            }
            set
            {
                _totalPages = value;
                NotifyPropertyChanged();
            }
        }

        private IEnumerable<int>? _availablePagesCollection;
        public IEnumerable<int>? AvailablePagesCollection
        {
            get
            {
                return _availablePagesCollection;
            }
            set
            {
                _availablePagesCollection = value;
                NotifyPropertyChanged();
            }
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
                return (_billingPerdiod?.IsOpen ?? false) ? new Dictionary<string, object>() : new Dictionary<string, object>() { { "disabled", "" } };
            }
        }

        public override async Task PageParameterSetAsync(Dictionary<string, object?> parameters)
        {
            IsBusy = true;
            _billingPerdiod = GetParameter<BillingPeriodReadDto>(parameters["BillingPeriod"]);
            _registerId = GetParameter<int>(parameters["RegisterId"]);
            CurrentPage = 1;
            TotalPages = 0;
            Entries = null;

            await ReadEntriesAsync(CancellationToken);
            await ReadCategoriesAsync(CancellationToken);
            IsBusy = false;
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
            if (EntryCreateDialog is null || _billingPerdiod is null)
            {
                return;
            }

            await EntryCreateDialog.InitializeDialogAsync(new EntryCreateDto());

            var result = await EntryCreateDialog.ShowModalAsync(CancellationToken);

            if (result is null)
            {
                return;
            }

            var creationResult = await _entryService.CreateEntryAsync(_registerId, _billingPerdiod.Id, result, CancellationToken);
            await ReadEntriesAsync(CancellationToken);

            if (RefreshChart is null)
            {
                return;
            }

            await RefreshChart();
        }

        public async Task DeleteEntry(EntryReadDto entryReadDto)
        {
            if (EntryDeleteDialog is null || _billingPerdiod is null)
                return;

            await EntryDeleteDialog.InitializeDialogAsync(entryReadDto);

            var result = await EntryDeleteDialog.ShowModalAsync(CancellationToken);

            if (result == ModalResult.Cancel)
                return;

            await _entryService.DeleteEntryAsync(_registerId, _billingPerdiod.Id, entryReadDto.Id, CancellationToken);
            await ReadEntriesAsync(CancellationToken);
        }

        private async Task ReadCategoriesAsync(CancellationToken cancellationToken)
        {
            var result = await _categoriesService.GetCategoriesAsync();

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
            if (_billingPerdiod is null)
            {
                return;
            }

            var result = await _entryService.GetEntriesAsync(_registerId, _billingPerdiod.Id, CurrentPage);

            if (!result.Result)
                return;

            Entries = result.Value?.Result;
            CurrentPage = result.Value?.CurrentPage ?? 0;
            TotalPages = result.Value?.TotalPages ?? 0;
            CalculateAvailablePages();
        }
    }
}
