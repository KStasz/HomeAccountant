using HomeAccountant.Core.DTOs.BillingPeriod;
using HomeAccountant.Core.DTOs.Category;
using HomeAccountant.Core.DTOs.Entry;
using HomeAccountant.Core.Model;
using HomeAccountant.Core.Services;
using System.Drawing;

namespace HomeAccountant.Core.ViewModels
{
    public class RegisterPositionsViewModel : BaseViewModel
    {
        private readonly IEntryService _entryService;
        private readonly ICategoriesService _categoriesService;
        private BillingPeriodReadDto? _billingPerdiod;
        private int _registerId;
        private Color[] colorPallete = new Color[]
        {
            Color.Red,
            Color.Orange,
            Color.Yellow,
            Color.Green,
            Color.Blue,
            Color.Purple,
            Color.Magenta,
            Color.Pink
        };

        public RegisterPositionsViewModel(IEntryService entryService,
            ICategoriesService categoriesService)
        {
            _entryService = entryService;
            _categoriesService = categoriesService;
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

        public async Task InitializeAsync(int registerId, BillingPeriodReadDto? billingPeriod)
        {
            IsBusy = true;
            _billingPerdiod = billingPeriod;
            _registerId = registerId;
            CurrentPage = 1;
            TotalPages = 0;
            Entries = null;

            await ReadEntries();
            await ReadCategories();
            IsBusy = false;
        }

        public List<ChartDataset>? GetChartDataset()
        {
            if (_billingPerdiod is null)
            {
                return null;
            }

            var groupedEntries = Entries?
                .GroupBy(x => x.Category.Name)
                .Select((x, i) => new
                ChartValue(x.Key, (double)x.Sum(y => y.Price), colorPallete[i]));

            if (groupedEntries is null)
                return null;

            return new List<ChartDataset>()
            {
                new ChartDataset(_billingPerdiod!.Name!, groupedEntries)
            };
        }

        private void CalculateAvailablePages()
        {
            var pages = Enumerable.Range(1, TotalPages)
                .Chunk(5);
            AvailablePagesCollection = pages.FirstOrDefault(x => x.Contains(CurrentPage));
        }

        private async Task ReadEntries()
        {
            if (_billingPerdiod is null)
            {
                return;
            }

            var result = await _entryService.GetEntries(_registerId, _billingPerdiod.Id, CurrentPage);

            if (!result.Result)
                return;


            Entries = result.Value?.Result;
            CurrentPage = result.Value?.CurrentPage ?? 0;
            TotalPages = result.Value?.TotalPages ?? 0;
            CalculateAvailablePages();
        }

        public async Task SetPage(int page)
        {
            IsBusy = true;
            CurrentPage = page;
            await ReadEntries();
            IsBusy = false;
        }

        public async Task NextPage()
        {
            if (CurrentPage < TotalPages)
            {
                IsBusy = true;
                CurrentPage++;
                await ReadEntries();
                IsBusy = false;
            }
        }

        public async Task PreviousPage()
        {
            if (CurrentPage > 1)
            {
                IsBusy = true;
                CurrentPage--;
                await ReadEntries();
                IsBusy = false;
            }
        }

        private async Task ReadCategories()
        {
            var result = await _categoriesService.GetCategoriesAsync();

            if (!result.Result)
                return;

            AvailableCategories = result.Value;
        }

        public async Task CreateEntry()
        {
            if (EntryCreateDialog is null || _billingPerdiod is null)
            {
                return;
            }

            await EntryCreateDialog.InitializeDialogAsync(new EntryCreateDto());

            var result = await EntryCreateDialog.ShowModalAsync();

            if (result is null)
            {
                return;
            }

            var creationResult = await _entryService.CreateEntry(_registerId, _billingPerdiod.Id, result);
            await ReadEntries();
        }

        public async Task DeleteEntry(EntryReadDto entryReadDto)
        {
            if (EntryDeleteDialog is null || _billingPerdiod is null)
                return;

            await EntryDeleteDialog.InitializeDialogAsync(entryReadDto);

            var result = await EntryDeleteDialog.ShowModalAsync();

            if (result == ModalResult.Cancel)
                return;

            await _entryService.DeleteEntry(_registerId, _billingPerdiod.Id, entryReadDto.Id);
            await ReadEntries();
        }
    }
}
