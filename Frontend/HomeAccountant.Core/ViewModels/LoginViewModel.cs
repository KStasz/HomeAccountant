using HomeAccountant.Core.Authentication;
using HomeAccountant.Core.DTOs.Authentication;
using HomeAccountant.Core.Services;

namespace HomeAccountant.Core.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly JwtAuthenticationStateProvider _jwtAuthenticationStateProvider;
        private readonly IJsCodeExecutor _jsCodeExecutor;
        private readonly ITokenStorageAccessor _tokenStorage;

        public LoginViewModel(IAuthenticationService authenticationService,
            JwtAuthenticationStateProvider jwtAuthenticationStateProvider,
            IJsCodeExecutor jsCodeExecutor,
            ITokenStorageAccessor tokenStorage)
        {
            _authenticationService = authenticationService;
            _jwtAuthenticationStateProvider = jwtAuthenticationStateProvider;
            _jsCodeExecutor = jsCodeExecutor;
            _tokenStorage = tokenStorage;
            _loginData = new LoginDTO();
            _registerData = new RegisterUserDto();
        }

        private LoginDTO _loginData;
        public LoginDTO LoginData
        {
            get => _loginData; 
            set => SetValue(ref  _loginData, value);
        }

        private RegisterUserDto _registerData;
        public RegisterUserDto RegisterData
        {
            get => _registerData; 
            set  => SetValue(ref _registerData, value);
        }

        public async Task Login()
        {
            ClearErrorMessage();

            var result = await _authenticationService.LoginAsync(LoginData.Email!, LoginData.Password!);

            if (!result.Result)
            {
                if (result.Errors is null)
                {
                    SetErrorMessage("Wystpił błąd podczas logowania");
                    
                    return;
                }

                SetErrorMessage(result.Errors?.ToArray() ?? new string[0]);

                return;
            }

            if (result.Value is null)
            {
                SetErrorMessage("Wystpił błąd podczas logowania");

                return;
            }

            await _tokenStorage.SetTokenAsync(result.Value);
            _jwtAuthenticationStateProvider.StateChanged();
            
            await _jsCodeExecutor.ExecuteFunctionAsync("HideModal");
        }

        public void Logout()
        {
            _tokenStorage.RemoveTokenAsync();
            _jwtAuthenticationStateProvider?.StateChanged();

            ClearLoginData();
        }

        public async Task Register()
        {
            ClearErrorMessage();
            var result = await _authenticationService.RegisterAsync(
                RegisterData.Email!,
                RegisterData.UserName!,
                RegisterData.Password!);

            if (!result.Result)
            {
                if (result.Errors is null)
                {
                    SetErrorMessage("Wystąpił błąd podczas rejestracji");
                }

                SetErrorMessage(result.Errors?.ToArray() ?? new string[0]);
                
                return;
            }

            if (result.Value is null)
            {
                SetErrorMessage("Wystąpił błąd podczas rejestracji");
                
                return;
            }

            await _tokenStorage.SetTokenAsync(result.Value);
            _jwtAuthenticationStateProvider.StateChanged();

            await _jsCodeExecutor.ExecuteFunctionAsync("HideModal");
        }

        private void ClearLoginData()
        {
            LoginData = new LoginDTO();
        }
    }
}

