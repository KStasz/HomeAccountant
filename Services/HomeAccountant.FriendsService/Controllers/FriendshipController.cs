using AutoMapper;
using Domain.Controller;
using Domain.Dtos.FriendsService;
using Domain.Dtos.IdentityPlatform;
using Domain.Model;
using Domain.Services;
using HomeAccountant.FriendsService.Data;
using HomeAccountant.FriendsService.Model;
using HomeAccountant.FriendsService.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace HomeAccountant.FriendsService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class FriendshipController : ServiceControllerBase
    {
        private readonly IRepository<ApplicationDbContext, Friendships> _friendshipRepository;
        private readonly IIdentityPlatformService _identityPlatformService;
        private readonly IMapper _mapper;
        private readonly ILogger<FriendshipController> _logger;

        public FriendshipController(
            IRepository<ApplicationDbContext, Friendships> friendshipRepository,
            IIdentityPlatformService identityPlatformService,
            IMapper mapper,
            ILogger<FriendshipController> logger)
        {
            _friendshipRepository = friendshipRepository;
            _identityPlatformService = identityPlatformService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<ServiceResponse<IEnumerable<FriendshipReadDto>?>>> GetFriendships(string? email = null, string? name = null)
        {
            try
            {
                List<FriendshipReadDto> usersOutput = new List<FriendshipReadDto>();

                if (UserId is null)
                    return BadRequest(new ServiceResponse(
                        new List<string>()
                        {
                            "Invalid payload"
                        }));

                var friendships = _friendshipRepository.GetAll(x => x.UserId == UserId);

                string[] userIdentifiers = new HashSet<string>(
                    friendships
                    .Select(x => x.FriendId)
                    .Concat(
                        friendships
                        .Select(x => x.UserId))
                    .Concat(
                        friendships
                        .Select(x => x.CreatorId))).ToArray();

                ServiceResponse<UserModelDto[]?> usersResponse = await _identityPlatformService.GetUsersAsync(userIdentifiers);

                foreach (var item in friendships)
                {
                    if (!usersResponse.Result)
                        continue;

                    usersOutput.Add(new FriendshipReadDto()
                    {
                        Id = item.Id,
                        User = usersResponse.Value?.FirstOrDefault(x => x.Id == item.UserId),
                        CreatedBy = usersResponse.Value?.FirstOrDefault(x => x.Id == item.CreatorId),
                        Friend = usersResponse.Value?.FirstOrDefault(x => x.Id == item.FriendId),
                        CreationDate = item.CreationDate,
                        IsAccepted = item.IsAccepted
                    });
                }

                return Ok(new ServiceResponse<IEnumerable<FriendshipReadDto>?>(usersOutput));
            }
            catch (Exception ex)
            {
                _logger.LogError($"--> {ex.Message}");

                return BadRequest(new ServiceResponse("Reading friendships failed"));
            }
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<ServiceResponse<IEnumerable<UserModelDto>?>>> GetFriends()
        {
            try
            {
                if (UserId is null)
                    return BadRequest(
                        new ServiceResponse<IEnumerable<UserModelDto>?>(
                            new string[] { "Invalid payload" }));

                var friendships = _friendshipRepository.GetAll(x => x.UserId == UserId);

                if (!friendships.Any())
                    return NotFound(
                        new ServiceResponse<IEnumerable<UserModelDto>?>(
                            new string[] { "Friends not found" }));

                var usersResponse = await _identityPlatformService.GetUsersAsync(
                    friendships.Select(x => x.FriendId).ToArray());

                if (usersResponse is null)
                    return BadRequest(new ServiceResponse<IEnumerable<UserModelDto>?>(
                            new string[] { "Unable to read users" }));

                if (!usersResponse.Result)
                    return BadRequest(
                        new ServiceResponse<IEnumerable<UserModelDto>?>(
                            usersResponse.Errors ?? Array.Empty<string>()));

                return Ok(
                    new ServiceResponse<IEnumerable<UserModelDto>?>(
                        usersResponse.Value));
            }
            catch (Exception ex)
            {
                _logger.LogError($"--> {ex.Message}");

                return BadRequest(
                        new ServiceResponse<IEnumerable<UserModelDto>?>(
                            new string[] { "Reading friends failed" }));
            }
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<ServiceResponse>> CreateFriendshipRequest([FromBody] string recipientEmail)
        {
            try
            {
                if (UserId is null)
                    return BadRequest(new ServiceResponse(
                        new List<string>()
                        {
                            "Invalid payload"
                        }));

                var ownerId = await _identityPlatformService.GetUserIdByEmailAsync(recipientEmail);

                if (ownerId is null)
                    return NotFound(new ServiceResponse(
                        new List<string>()
                        {
                            "User with specified email doesn't exist"
                        }));

                var senderEmailResponse = await _identityPlatformService.GetEmailByUserId(UserId);

                if (!senderEmailResponse.Result)
                    return BadRequest(senderEmailResponse);

                _friendshipRepository.Add(new Friendships()
                {
                    UserId = UserId,
                    FriendId = ownerId,
                    CreatorId = UserId
                });
                _friendshipRepository.Add(new Friendships()
                {
                    UserId = ownerId,
                    FriendId = UserId,
                    CreatorId = UserId
                });

                await _friendshipRepository.SaveChangesAsync();

                return Ok(new ServiceResponse(true));
            }
            catch (Exception ex)
            {
                _logger.LogError($"{Environment.NewLine}--> {ex.Message}{Environment.NewLine}");

                return BadRequest(
                    new ServiceResponse(
                        new List<string>()
                        {
                            "Unable to create friendship request"
                        }));
            }
        }

        [HttpDelete]
        public async Task<ActionResult<ServiceResponse>> DeleteFriendship(int friendshipId)
        {
            try
            {
                var friendship = _friendshipRepository.Get(x => x.Id == friendshipId);

                if (friendship is null)
                    return NotFound(new ServiceResponse("Unable to find specified friend"));

                var secondFriendship = _friendshipRepository.Get(x => x.UserId == friendship.FriendId);

                if (secondFriendship is null)
                    return NotFound(new ServiceResponse("Unable to find specified friend"));

                _friendshipRepository.RemoveMany(new Friendships[] { friendship, secondFriendship });
                await _friendshipRepository.SaveChangesAsync();

                return Ok(new ServiceResponse(true));

            }
            catch (Exception ex)
            {
                _logger.LogError($"--> {ex.Message}");

                return BadRequest(
                    new ServiceResponse(
                        "Friendship delete failed"));
            }
        }

        [HttpPut("[Action]")]
        public async Task<ActionResult<ServiceResponse>> AcceptFriendship(int friendshipId)
        {
            try
            {
                if (UserId is null)
                    return BadRequest(new ServiceResponse("Invaild payload"));

                var friendship = _friendshipRepository.Get(x => x.Id == friendshipId);

                if (friendship is null)
                    return NotFound(new ServiceResponse("Unable to find specified friendship"));

                if (friendship.CreatorId == UserId)
                    return BadRequest(new ServiceResponse("You are not allowed to accept this friendship"));

                var secondFriendship = _friendshipRepository.Get(x => x.UserId == friendship.FriendId);

                if (secondFriendship is null)
                    return NotFound(new ServiceResponse("Unable to find specified friendship"));

                friendship.IsAccepted = true;
                secondFriendship.IsAccepted = true;

                _friendshipRepository.Update(friendship);
                _friendshipRepository.Update(secondFriendship);
                await _friendshipRepository.SaveChangesAsync();

                return Ok(new ServiceResponse(true));
            }
            catch (Exception ex)
            {
                _logger.LogError($"--> {ex.Message}");

                return BadRequest(new ServiceResponse("Accepting friendship failed"));
            }
        }
    }
}
