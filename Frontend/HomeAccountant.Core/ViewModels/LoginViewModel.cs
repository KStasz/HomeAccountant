using HomeAccountant.Core.Authentication;
using HomeAccountant.Core.DTOs.Authentication;
using HomeAccountant.Core.Mapper;
using HomeAccountant.Core.Model;
using HomeAccountant.Core.Services;

namespace HomeAccountant.Core.ViewModels
{
    public class LoginViewModel : MvvmViewModel
    {
        private readonly JwtAuthenticationStateProvider _jwtAuthenticationStateProvider;
        private readonly ITokenStorageAccessor _tokenStorage;
        private readonly ITypeMapper<TokenAuthenticationModel, LoginResponseDTO> _mapper;

        public LoginViewModel(IAuthenticationService authenticationService,
            JwtAuthenticationStateProvider jwtAuthenticationStateProvider,
            ITokenStorageAccessor tokenStorage,
            ITypeMapper<TokenAuthenticationModel, LoginResponseDTO> mapper)
        {
            AuthenticationService = authenticationService;
            _jwtAuthenticationStateProvider = jwtAuthenticationStateProvider;
            _tokenStorage = tokenStorage;
            _mapper = mapper;
        }

        public IAuthenticationService AuthenticationService { get; private set; }
        public IModalDialog<LoginDTO, LoginResponseDTO>? LoginDialog { get; set; }
        public IModalDialog<RegisterUserDto, LoginResponseDTO>? RegisterDialog { get; set; }

        public async Task Login()
        {
            if (LoginDialog is null)
                return;

            await LoginDialog.InitializeDialogAsync(new LoginDTO());

            var result = await LoginDialog.ShowModalAsync();

            if (result is null)
                return;

            await LoginDialog.HideModalAsync();

            var tokenAuthentication = _mapper.Map(result);

            await _tokenStorage.SetTokenAsync(tokenAuthentication);
            _jwtAuthenticationStateProvider.StateChanged();
        }

        public void Logout()
        {
            _tokenStorage.RemoveTokenAsync();
            _jwtAuthenticationStateProvider?.StateChanged();
        }

        public async Task Register()
        {
            if (RegisterDialog is null)
                return;

            await RegisterDialog.InitializeDialogAsync(new RegisterUserDto());

            var result = await RegisterDialog.ShowModalAsync();

            if (result is null)
                return;

            await RegisterDialog.HideModalAsync();

            var tokenAuthentication = _mapper.Map(result);

            await _tokenStorage.SetTokenAsync(tokenAuthentication);
            _jwtAuthenticationStateProvider.StateChanged();
        }
    }
}

