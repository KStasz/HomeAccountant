using HomeAccountant.Core.Authentication;
using HomeAccountant.Core.DTOs.Authentication;
using HomeAccountant.Core.Mapper;
using HomeAccountant.Core.Model;
using HomeAccountant.Core.Services;
using Microsoft.AspNetCore.Components;

namespace HomeAccountant.Core.ViewModels
{
    public class LoginViewModel : MvvmViewModel
    {
        private readonly JwtAuthenticationStateProvider _jwtAuthenticationStateProvider;
        private readonly ITokenStorageAccessor _tokenStorage;
        private readonly NavigationManager _navManager;

        public LoginViewModel(IAuthenticationService authenticationService,
            JwtAuthenticationStateProvider jwtAuthenticationStateProvider,
            ITokenStorageAccessor tokenStorage,
            NavigationManager navManager)
        {
            AuthenticationService = authenticationService;
            _jwtAuthenticationStateProvider = jwtAuthenticationStateProvider;
            _tokenStorage = tokenStorage;
            _navManager = navManager;
        }

        public IAuthenticationService AuthenticationService { get; private set; }
        public IModalDialog<LoginModel, LoginResponseModel>? LoginDialog { get; set; }
        public IModalDialog<RegisterUserModel, LoginResponseModel>? RegisterDialog { get; set; }

        public async Task Login()
        {
            if (LoginDialog is null)
                return;

            await LoginDialog.InitializeDialogAsync(new LoginModel());

            var result = await LoginDialog.ShowModalAsync();

            if (result is null)
                return;

            await LoginDialog.HideModalAsync();

            await _tokenStorage.SetTokenAsync(result);
            _jwtAuthenticationStateProvider.StateChanged();
        }

        public void Logout()
        {
            _tokenStorage.RemoveTokenAsync();
            _jwtAuthenticationStateProvider?.StateChanged();
            _navManager.NavigateTo("/", false);
        }

        public async Task Register()
        {
            if (RegisterDialog is null)
                return;

            await RegisterDialog.InitializeDialogAsync(new RegisterUserModel());

            var result = await RegisterDialog.ShowModalAsync();

            if (result is null)
                return;

            await RegisterDialog.HideModalAsync();

            await _tokenStorage.SetTokenAsync(result);
            _jwtAuthenticationStateProvider.StateChanged();
        }
    }
}

