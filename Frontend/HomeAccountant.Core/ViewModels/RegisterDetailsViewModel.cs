using HomeAccountant.Core.DTOs.Category;
using HomeAccountant.Core.DTOs.Entry;
using HomeAccountant.Core.Model;
using HomeAccountant.Core.Services;

namespace HomeAccountant.Core.ViewModels
{
    public class RegisterPositionsViewModel : BaseViewModel
    {
        private readonly IEntryService _entryService;
        private readonly ICategoriesService _categoriesService;
        private int _billingPerdiodId;
        private int _registerId;

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

        private bool _isBillingPeriodOpen;
        public bool IsBillingPeriodOpen
        {
            get 
            {
                return _isBillingPeriodOpen; 
            }
            set 
            {
                _isBillingPeriodOpen = value;
                NotifyPropertyChanged();
            }
        }

        public Dictionary<string, object> IsButtonBlocked
        {
            get
            {
                return IsBillingPeriodOpen ? new Dictionary<string, object>() : new Dictionary<string, object>() { { "disabled", "" } };
            }
        }

        public async Task InitializeAsync(int registerId, int billingPeriodId)
        {
            IsBusy = true;
            _billingPerdiodId = billingPeriodId;
            _registerId = registerId;

            await ReadEntries();
            await ReadCategories();
            IsBusy = false;
        }

        private async Task ReadEntries()
        {
            var result = await _entryService.GetEntries(_registerId, _billingPerdiodId);

            if (!result.Result)
                return;

            Entries = result.Value;
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
            if (EntryCreateDialog is null)
            {
                return;
            }

            await EntryCreateDialog.InitializeDialogAsync(new EntryCreateDto());

            var result = await EntryCreateDialog.ShowModalAsync();

            if (result is null)
            {
                return;
            }

            var creationResult = await _entryService.CreateEntry(_registerId, _billingPerdiodId, result);
            await ReadEntries();
        }

        public async Task DeleteEntry(EntryReadDto entryReadDto)
        {
            if (EntryDeleteDialog is null)
                return;

            await EntryDeleteDialog.InitializeDialogAsync(entryReadDto);

            var result = await EntryDeleteDialog.ShowModalAsync();

            if (result == ModalResult.Cancel)
                return;

            await _entryService.DeleteEntry(_registerId, _billingPerdiodId, entryReadDto.Id);
            await ReadEntries();
        }
    }
}
