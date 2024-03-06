using HomeAccountant.Core.Authentication;
using HomeAccountant.Core.Extensions;
using HomeAccountant.Core.Model;
using HomeAccountant.Core.Services;
using System.Security.Claims;

namespace HomeAccountant.Core.ViewModels
{
    public class FriendsPanelViewModel : MvvmViewModel
    {
        private readonly IFriendshipService _friendsService;
        private readonly JwtAuthenticationStateProvider _jwtAuthenticationStateProvider;

        public FriendsPanelViewModel(
            IFriendshipService friendsService,
            JwtAuthenticationStateProvider jwtAuthenticationStateProvider)
        {
            _friendsService = friendsService;
            _jwtAuthenticationStateProvider = jwtAuthenticationStateProvider;
        }

        public IAlert? Alert { get; set; }

        public IModalDialog<string, string>? CreateInvitationDialog;

        public IModalDialog<FriendshipModel>? DeleteFriendshipDialog;

        private ServiceResponse<IEnumerable<FriendshipModel>?>? _friendsCollection;
        public ServiceResponse<IEnumerable<FriendshipModel>?>? FriendsCollection
        {
            get => _friendsCollection;
            set => SetValue(ref _friendsCollection, value);
        }

        private string? _currentUserIdentifier;
        public string? CurrentUserIdentifier
        {
            get => _currentUserIdentifier;
            set => SetValue(ref _currentUserIdentifier, value);
        }

        public override async Task PageInitializedAsync()
        {
            IsBusy = true;

            await ReadFriendshipCollection();
            await ReadUserIdentifier();

            IsBusy = false;
        }

        private async Task ReadUserIdentifier()
        {
            var state = await _jwtAuthenticationStateProvider.GetAuthenticationStateAsync();
            CurrentUserIdentifier = (state.User.Identity as ClaimsIdentity)?.Claims
                .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        }

        public async Task CreateFriendship()
        {
            if (CreateInvitationDialog is null)
                return;

            await CreateInvitationDialog.InitializeDialogAsync(string.Empty);

            var recipientEmail = await CreateInvitationDialog.ShowModalAsync();

            if (recipientEmail is null)
                return;

            var response = await _friendsService.CreateFriendship(recipientEmail, CancellationToken);

            if (!response.Result)
            {
                if (Alert is null)
                    return;

                await Alert.ShowAlertAsync(
                    response.Errors.JoinToString(),
                    AlertType.Danger,
                    CancellationToken);

                return;
            }

            await ReadFriendshipCollection();
        }

        public async Task DeleteFriendship(FriendshipModel friendship)
        {
            if (DeleteFriendshipDialog is null)
                return;

            await DeleteFriendshipDialog.InitializeDialogAsync(friendship);
            var result = await DeleteFriendshipDialog.ShowModalAsync(CancellationToken);

            if (result == ModalResult.Cancel)
                return;

            var response = await _friendsService.DeleteFriendship(friendship.Id);

            if (!response.Result)
            {
                if (Alert is null)
                    return;

                await Alert.ShowAlertAsync(response.Errors.JoinToString(), AlertType.Danger, CancellationToken);
            }

            await ReadFriendshipCollection();
        }

        public async Task AcceptFriendship(FriendshipModel friendshipModel)
        {
            await _friendsService.AcceptFriendship(friendshipModel.Id);
            await ReadFriendshipCollection();
        }

        private async Task ReadFriendshipCollection(string? email = null, string? name = null)
        {
            FriendsCollection = await _friendsService.GetFriendships(email, name);
        }
    }
}
