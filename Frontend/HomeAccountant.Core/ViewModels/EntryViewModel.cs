using HomeAccountant.Core.Model;
using HomeAccountant.Core.Services;

namespace HomeAccountant.Core.ViewModels
{
    public class EntryViewModel : MvvmViewModel
    {
        private readonly IEntryService _entryService;
        private readonly ICategoriesService _categoriesService;
        private readonly IPubSubService _pubSubService;
        private readonly IEntriesRealTimeService _entriesRealTimeService;
        private int _billingPerdiodId;
        private int _registerId;
        private bool _isPeriodOpen;

        public EntryViewModel(IEntryService entryService,
            ICategoriesService categoriesService,
            IPubSubService pubSubService,
            IEntriesRealTimeService entriesRealTimeService)
        {
            _entryService = entryService;
            _categoriesService = categoriesService;
            _pubSubService = pubSubService;
            _entriesRealTimeService = entriesRealTimeService;
            _entriesRealTimeService.EntryCreated += EntriesRealTimeService_EntryCreated;
        }

        public IModalDialog<EntryModel, EntryModel>? EntryCreateDialog { get; set; }
        public IModalDialog<EntryModel>? EntryDeleteDialog { get; set; }

        private IEnumerable<EntryModel>? _entries;
        public IEnumerable<EntryModel>? Entries
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

        public override async Task PageParameterSetAsync(Dictionary<string, object?> parameters)
        {
            IsBusy = true;
            _billingPerdiodId = GetParameter<int>(parameters["BillingPeriodId"]);
            _registerId = GetParameter<int>(parameters["RegisterId"]);
            _isPeriodOpen = GetParameter<bool>(parameters["IsPeriodOpen"]);
            CurrentPage = 1;
            TotalPages = 0;
            Entries = null;

            await ReadEntriesAsync(CancellationToken);
            await ReadCategoriesAsync(CancellationToken);
            await _entriesRealTimeService.InitializeAsync(CancellationToken);
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

            var creationResult = await _entryService.CreateEntryAsync(_registerId, _billingPerdiodId, result, CancellationToken);

            await _entriesRealTimeService.EntryCreatedAsync(CancellationToken);
            //await _pubSubService.Send(this);
        }

        public async Task DeleteEntry(EntryModel entryReadDto)
        {
            if (EntryDeleteDialog is null)
                return;

            await EntryDeleteDialog.InitializeDialogAsync(entryReadDto);

            var result = await EntryDeleteDialog.ShowModalAsync(CancellationToken);

            if (result == ModalResult.Cancel)
                return;

            await _entryService.DeleteEntryAsync(_registerId, _billingPerdiodId, entryReadDto.Id, CancellationToken);

            await _entriesRealTimeService.EntryCreatedAsync(CancellationToken);
            //await _pubSubService.Send(this);
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
            var result = await _entryService.GetEntriesAsync(_registerId, _billingPerdiodId, CurrentPage, cancellationToken: cancellationToken);

            if (!result.Result)
                return;

            Entries = result.Value?.Result;
            CurrentPage = result.Value?.CurrentPage ?? 0;
            TotalPages = result.Value?.TotalPages ?? 0;
            CalculateAvailablePages();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        protected override async ValueTask DisposeAsyncCore()
        {
            _entriesRealTimeService.EntryCreated -= EntriesRealTimeService_EntryCreated;

            await _entriesRealTimeService.DisposeAsync().ConfigureAwait(false);
            await base.DisposeAsyncCore();
        }

        private async Task EntriesRealTimeService_EntryCreated(object sender, RealTimeEventArgs e)
        {
            await _pubSubService.Send(this);
        }
    }
}
