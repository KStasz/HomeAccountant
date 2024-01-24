using Domain.Controller;
using Domain.Dtos.IdentityPlatform;
using Domain.Model;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HomeAccountant.IdentityPlatform.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountInfoController : ServiceControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;

        public AccountInfoController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<ServiceResponse<UserUsernameReadDto[]>>> GetUserNames(string[] userIds)
        {
            UserUsernameReadDto[] result = new UserUsernameReadDto[userIds.Length];

            for (int i = 0; i < result.Length; i++)
            {
                var user = await _userManager.FindByIdAsync(userIds[i]);

                if (user is null)
                    continue;

                result[i] = new UserUsernameReadDto()
                {
                    UserId = user.Id,
                    UserName = user.UserName ?? string.Empty
                };
            }

            return Ok(new ServiceResponse<UserUsernameReadDto[]>(result));
        }
    }
}
