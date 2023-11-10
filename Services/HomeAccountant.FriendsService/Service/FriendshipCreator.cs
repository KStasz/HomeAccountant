using AutoMapper;
using HomeAccountant.FriendsService.Model;

namespace HomeAccountant.FriendsService.Service
{
    public class FriendshipCreator : IFriendshipCreator
    {
        private readonly IFriendsService _friendsService;
        private readonly IMapper _mapper;

        public FriendshipCreator(IFriendsService friendsService,
            IMapper mapper)
        {
            _friendsService = friendsService;
            _mapper = mapper;
        }

        public void CreateFriendship(FriendRequest request)
        {
            var reverseRequest = ReverseRequest(request);

            var friendship = _mapper.Map<Friend>(request);
            var reversedFriendship = _mapper.Map<Friend>(reverseRequest);

            _friendsService.CreateFriendship(friendship);
            _friendsService.CreateFriendship(reversedFriendship);
        }

        private FriendRequest ReverseRequest(FriendRequest friendRequest)
        {
            var reversedRequest = new FriendRequest()
            {
                CreatorId = friendRequest.RecipientId,
                RecipientId = friendRequest.CreatorId,
            };

            return reversedRequest;
        }
    }
}
