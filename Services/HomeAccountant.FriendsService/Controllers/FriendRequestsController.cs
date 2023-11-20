using AutoMapper;
using Domain.Dtos.FriendsService;
using HomeAccountant.FriendsService.Model;
using HomeAccountant.FriendsService.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeAccountant.FriendsService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class FriendRequestsController : ControllerBase
    {
        private readonly IFriendRequestsService _friendRequestsService;
        private readonly IMapper _mapper;
        private readonly IIdentityPlatformService _identityPlatformService;
        private readonly IFriendshipCreator _friendshipCreator;
        private readonly IFriendsService _friendsService;

        public FriendRequestsController(IFriendRequestsService friendRequestsService,
            IMapper mapper,
            IIdentityPlatformService identityPlatformService,
            IFriendshipCreator friendshipCreator,
            IFriendsService friendsService)
        {
            _friendRequestsService = friendRequestsService;
            _mapper = mapper;
            _identityPlatformService = identityPlatformService;
            _friendshipCreator = friendshipCreator;
            _friendsService = friendsService;
        }

        private string? GetUserId() => this.User.Claims.FirstOrDefault(x => x.Type == "UserId")?.Value;

        [HttpGet("CreatedRequests")]
        public IActionResult GetCreatedRequests()
        {
            var userId = GetUserId();

            if (userId is null)
            {
                return BadRequest("Invalid payload");
            }

            var requestsResponse = _friendRequestsService.GetRequests(x => x.CreatorId == userId);

            if (requestsResponse is null)
            {
                return Empty;
            }

            var responses = _mapper.Map<IEnumerable<FriendResponseDto>>(requestsResponse);

            return Ok(requestsResponse);
        }

        [HttpGet]
        public IActionResult GetUserRequests()
        {
            var userId = GetUserId();

            if (userId is null)
            {
                return BadRequest("Invalid payload");
            }

            var requestsResponse = _friendRequestsService.GetRequests(x => x.RecipientId == userId);

            if (requestsResponse is null)
            {
                return NotFound();
            }

            var responses = _mapper.Map<IEnumerable<FriendResponseDto>>(requestsResponse);

            return Ok(responses);
        }

        [HttpPut("{requestId}/Accept")]
        public async Task<IActionResult> AcceptRequest(int requestId)
        {
            var userId = GetUserId();

            if (userId is null)
                return BadRequest("Invalid payload");

            var request = _friendRequestsService.GetRequest(requestId);

            if (request is null)
                return NotFound();
            
            if (request.CreatorId == userId)
                return BadRequest("You can't accept request created by you");

            _friendshipCreator.CreateFriendship(request);
            _friendRequestsService.DeleteRequest(request);
            await _friendRequestsService.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("{requestId}/Reject")]
        public async Task<IActionResult> RejectRequest(int requestId)
        {
            var userId = GetUserId();

            if (userId is null)
                return BadRequest("Invalid payload");

            var request = _friendRequestsService.GetRequest(requestId);

            if (request is null)
            {
                return NotFound();
            }

            if (request.CreatorId == userId)
                return BadRequest("You can't reject request creted by you");

            request.IsRejected = true;

            _friendRequestsService.UpdateRequest(request);
            await _friendRequestsService.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("{requestId}", Name = "GetRequest")]
        public IActionResult GetRequest(int requestId)
        {
            var request = _friendRequestsService.GetRequest(requestId);

            if (request is null)
            {
                return NotFound();
            }

            var response = _mapper.Map<FriendResponseDto>(request);

            return Ok(response);
        }

        [HttpPost("CreateRequest")]
        public async Task<IActionResult> CreateRequest(CreateFriendRequestDto createFriendRequestDto)
        {
            var userId = GetUserId();
            var recipientId = await _identityPlatformService.GetUserIdByEmailAsync(createFriendRequestDto.RecipientEmail);

            var existingRequest = _friendRequestsService.SearchRequest(x => x.CreatorId == userId
                && x.RecipientId == recipientId
                && x.IsRejected == false);

            if (existingRequest is not null)
                return BadRequest("Request exists already");

            if (userId is null || recipientId is null)
                return BadRequest("Invalid payload");
            
            if (!ModelState.IsValid)
                return BadRequest("Invalid payload");
            
            if (userId == recipientId)
                return BadRequest("Creator cannot be the same as recipient");
            
            var existingFriendShip = _friendsService.GetFriend(x => x.UserId == userId && x.FriendId == recipientId);

            if (existingFriendShip is not null)
                return BadRequest("Cannot send a request because you are friends already");
            
            var friendRequest = new FriendRequest()
            {
                CreatorId = userId,
                RecipientId = recipientId
            };

            try
            {
                _friendRequestsService.CreateRequest(friendRequest);

                await _friendRequestsService.SaveChangesAsync();

                var response = _mapper.Map<FriendResponseDto>(friendRequest);

                return CreatedAtRoute(nameof(GetRequest), new { requestId = response.Id }, response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Problem durring creating friend request {ex.Message}");

                return BadRequest("Error occurred durring creating friend request");
            }
        }
    }
}