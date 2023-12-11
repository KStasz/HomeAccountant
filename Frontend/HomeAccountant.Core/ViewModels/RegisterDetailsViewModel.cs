using HomeAccountant.Core.DTOs;
using HomeAccountant.Core.DTOs.Category;
using HomeAccountant.Core.Model;
using HomeAccountant.Core.Services;

namespace HomeAccountant.Core.ViewModels
{
    public class RegisterPositionsViewModel : BaseViewModel
    {
        private readonly IEntryService _entryService;
        private readonly ICategoriesService _categoriesService;
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


        public async Task InitializeAsync(int registerId)
        {
            _registerId = registerId;

            await ReadEntries();
            await ReadCategories();
        }

        private async Task ReadEntries()
        {
            ServiceResponse<IEnumerable<EntryReadDto>?> result = await _entryService.GetEntries(_registerId);

            if (!result.IsSucceed)
                return;

            Entries = result.Result;
        }

        private async Task ReadCategories()
        {
            var result = await _categoriesService.GetCategoriesAsync();

            if (!result.IsSucceed)
                return;

            AvailableCategories = result.Result;
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

            var creationResult = await _entryService.CreateEntry(_registerId, result);
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

            await _entryService.DeleteEntry(_registerId, entryReadDto.Id);
            await ReadEntries();
        }
    }
}
