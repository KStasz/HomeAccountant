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
    public class FriendsController : ServiceControllerBase
    {
        private readonly IRepository<ApplicationDbContext, Friend> _friendsService;
        private readonly IRepository<ApplicationDbContext, FriendRequest> _friendRequestsService;
        private readonly IFriendshipCreator _friendshipCreator;
        private readonly IIdentityPlatformService _identityPlatformService;

        public FriendsController(IRepository<ApplicationDbContext, Friend> friendsService,
            IRepository<ApplicationDbContext, FriendRequest> friendRequestsService,
            IFriendshipCreator friendshipCreator,
            IIdentityPlatformService identityPlatformService)
        {
            _friendsService = friendsService;
            _friendRequestsService = friendRequestsService;
            _friendshipCreator = friendshipCreator;
            _identityPlatformService = identityPlatformService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateFriendship(CreateFriendshipDto createFriendRequest)
        {
            var friendRequest = _friendRequestsService.Get(x => x.Id == createFriendRequest.FriendRequestId);

            if (friendRequest is null)
            {
                return NotFound();
            }
            try
            {
                _friendshipCreator.CreateFriendship(friendRequest);
                await _friendsService.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                return BadRequest("An error occurred during friendship create");
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteFriendship(int id)
        {
            var friend = _friendsService.Get(x => x.Id == id);

            if (friend is null)
            {
                return NotFound();
            }

            _friendsService.Remove(friend);

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetFriends([FromQuery] string? email, string? name)
        {
            var userId = this.User.Claims.FirstOrDefault(x => x.Type == "UserId")?.Value;

            if (userId is null) 
            {
                return BadRequest("Invalid payload");
            }

            var friends = _friendsService.GetAll(x => x.UserId == userId);

            var users = await _identityPlatformService.GetUsersAsync(friends.Select(x => x.FriendId).ToArray());

            return Ok(users?.Where(x => 
                x.Email
                .ToLower()
                .Contains(email?.ToLower() ?? string.Empty)
                && x.UserName
                .ToLower()
                .Contains(name?.ToLower() ?? string.Empty)));
        }
    }
}
