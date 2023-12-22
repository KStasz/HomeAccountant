using AutoMapper;
using Domain.Services;
using HomeAccountant.FriendsService.Data;
using HomeAccountant.FriendsService.Model;

namespace HomeAccountant.FriendsService.Service
{
    public class FriendshipCreator : IFriendshipCreator
    {
        private readonly IRepository<ApplicationDbContext, Friend> _friendsService;
        private readonly IMapper _mapper;

        public FriendshipCreator(IRepository<ApplicationDbContext, Friend> friendsService,
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

            _friendsService.Add(friendship);
            _friendsService.Add(reversedFriendship);
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
