using AutoMapper;
using HomeAccountant.FriendsService.Dtos;
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

        public FriendRequestsController(IFriendRequestsService friendRequestsService,
            IMapper mapper)
        {
            _friendRequestsService = friendRequestsService;
            _mapper = mapper;
        }

        [HttpGet("CreatedRequests/{creatorId}")]
        public IActionResult GetCreatedRequests(string creatorId)
        {
            var requests = _friendRequestsService.GetRequests(x => x.CreatorId == creatorId);

            if (requests is null)
            {
                return NotFound();
            }

            var responses = _mapper.Map<IEnumerable<FriendResponseDto>>(requests);

            return Ok(requests);
        }

        [HttpGet("UserRequests/{userId}")]
        public IActionResult GetUserRequests(string userId)
        {
            var requests = _friendRequestsService.GetRequests(x => x.RecipientId == userId);

            if (requests is null)
            {
                return NotFound();
            }

            var responses = _mapper.Map<IEnumerable<FriendResponseDto>>(requests);

            return Ok(responses);
        }

        [HttpPut("Accept")]
        public IActionResult AcceptRequest(ManageFriendRequestDto manageFriendRequestDto)
        {
            // TODO

            return Ok();
        }

        [HttpPut("Reject")]
        public async Task<IActionResult> RejectRequest(ManageFriendRequestDto manageFriendRequestDto)
        {
            var request = _friendRequestsService.GetRequest(manageFriendRequestDto.RequestId);

            if (request is null)
            {
                return NotFound();
            }

            request.IsRejected = true;

            _friendRequestsService.UpdateRequest(request);
            await _friendRequestsService.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("Get/{requestId}", Name = "GetRequest")]
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
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid payload");
            }

            if (createFriendRequestDto.CreatorId == createFriendRequestDto.RecipientId)
            {
                return BadRequest("Creator cannot be the same as recipient");
            }

            var friendRequest = _mapper.Map<FriendRequest>(createFriendRequestDto);

            _friendRequestsService.CreateRequest(friendRequest);

            await _friendRequestsService.SaveChangesAsync();

            var response = _mapper.Map<FriendResponseDto>(friendRequest);

            return CreatedAtRoute(nameof(GetRequest), new { requestId = response.Id }, response);
        }
    }
}