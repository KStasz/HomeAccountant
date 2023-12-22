using HomeAccountant.Core.ViewModels;
using Microsoft.AspNetCore.Components;

namespace HomeAccountant.Pages
{
    public abstract class MvvmComponent<T> : ComponentBase, IDisposable where T : MvvmViewModel
    {
        [Inject]
        public required T ViewModel { get; set; }

        protected override void OnInitialized()
        {
            ViewModel.PropertyChangedAsync += ViewModel_PropertyChangedAsync;
            ViewModel.PageInitialized();
        }

        protected override Task OnInitializedAsync()
        {
            return ViewModel.PageInitializedAsync();
        }

        private async Task ViewModel_PropertyChangedAsync(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            await InvokeAsync(StateHasChanged);
        }

        protected override Task OnParametersSetAsync()
        {
            Dictionary<string, object?> parameters = this
                .GetType()
                .GetProperties()
                .Where(x => Attribute.IsDefined(x, typeof(ParameterAttribute)))
                .Select(x => new
                {
                    x.Name,
                    Value = x.GetValue(this)
                })
                .ToDictionary(
                    x => x.Name,
                    x => x.Value);

            return ViewModel.PageParameterSetAsync(parameters);
        }

        protected override void OnAfterRender(bool firstRender)
        {
            ViewModel.PageRendered(firstRender);
        }

        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            return ViewModel.PageRenderedAsync(firstRender);
        }

        protected override void OnParametersSet()
        {
            Dictionary<string, object?> parameters = this
                .GetType()
                .GetProperties()
                .Where(x => Attribute.IsDefined(x, typeof(ParameterAttribute)))
                .Select(x => new
                {
                    x.Name,
                    Value = x.GetValue(this)
                })
                .ToDictionary(
                    x => x.Name,
                    x => x.Value);

            ViewModel.PageParameterSet(parameters);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            ViewModel.PropertyChangedAsync -= ViewModel_PropertyChangedAsync;
            ViewModel.Dispose();
        }
    }
}
