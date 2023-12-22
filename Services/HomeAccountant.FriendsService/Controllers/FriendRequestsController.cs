using AutoMapper;
using Domain.Controller;
using Domain.Dtos.FriendsService;
using Domain.Services;
using HomeAccountant.FriendsService.Data;
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
    public class FriendRequestsController : ServiceControllerBase
    {
        private readonly IRepository<ApplicationDbContext, FriendRequest> _friendRequestsService;
        private readonly IMapper _mapper;
        private readonly IIdentityPlatformService _identityPlatformService;
        private readonly IFriendshipCreator _friendshipCreator;
        private readonly IRepository<ApplicationDbContext, Friend> _friendsService;

        public FriendRequestsController(IRepository<ApplicationDbContext, FriendRequest> friendRequestsService,
            IMapper mapper,
            IIdentityPlatformService identityPlatformService,
            IFriendshipCreator friendshipCreator,
            IRepository<ApplicationDbContext, Friend> friendsService)
        {
            _friendRequestsService = friendRequestsService;
            _mapper = mapper;
            _identityPlatformService = identityPlatformService;
            _friendshipCreator = friendshipCreator;
            _friendsService = friendsService;
        }

        [HttpGet("CreatedRequests")]
        public IActionResult GetCreatedRequests()
        {
            var userId = GetUserId();

            if (userId is null)
            {
                return BadRequest("Invalid payload");
            }

            var requestsResponse = _friendRequestsService.Get(x => x.CreatorId == userId);

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

            var requestsResponse = _friendRequestsService.Get(x => x.RecipientId == userId);

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

            var request = _friendRequestsService.Get(x => x.Id == requestId);

            if (request is null)
                return NotFound();
            
            if (request.CreatorId == userId)
                return BadRequest("You can't accept request created by you");

            _friendshipCreator.CreateFriendship(request);
            _friendRequestsService.Remove(request);
            await _friendRequestsService.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("{requestId}/Reject")]
        public async Task<IActionResult> RejectRequest(int requestId)
        {
            var userId = GetUserId();

            if (userId is null)
                return BadRequest("Invalid payload");

            var request = _friendRequestsService.Get(x => x.Id == requestId);

            if (request is null)
            {
                return NotFound();
            }

            if (request.CreatorId == userId)
                return BadRequest("You can't reject request creted by you");

            request.IsRejected = true;

            _friendRequestsService.Update(request);
            await _friendRequestsService.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("{requestId}", Name = "GetRequest")]
        public IActionResult GetRequest(int requestId)
        {
            var request = _friendRequestsService.Get(x => x.Id == requestId);

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

            var existingRequest = _friendRequestsService.Get(x => x.CreatorId == userId
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
            
            var existingFriendShip = _friendsService.Get(x => x.UserId == userId && x.FriendId == recipientId);

            if (existingFriendShip is not null)
                return BadRequest("Cannot send a request because you are friends already");
            
            var friendRequest = new FriendRequest()
            {
                CreatorId = userId,
                RecipientId = recipientId
            };

            try
            {
                _friendRequestsService.Add(friendRequest);

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