using HomeAccountant.IdentityPlatform.Dtos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HomeAccountant.IdentityPlatform.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;

        public AccountController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpPut("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] UserPasswordChangeDto userPasswordChangeDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    Result = false,
                    Errors = new string[]
                    {
                        "Invalid payload"
                    }
                });
            }

            if (userPasswordChangeDto.CurrentPassword == userPasswordChangeDto.NewPassword)
            {
                return BadRequest(new
                {
                    Result = false,
                    Errors = new string[]
                    {
                        "Passwords cannot be the same"
                    }
                });
            }

            var user = await _userManager.FindByIdAsync(userPasswordChangeDto.UserId);

            if (user is null)
            {
                return NotFound(new
                {
                    Result = false,
                    Errors = new string[]
                    {
                        "User not found"
                    }
                });
            }

            var result = await _userManager.ChangePasswordAsync(user, userPasswordChangeDto.CurrentPassword, userPasswordChangeDto.NewPassword);

            if (!result.Succeeded)
            {
                return BadRequest(new
                {
                    Result = false,
                    Errors = result.Errors.Select(x => x.Description)
                });
            }

            return Ok();
        }
    }
}
