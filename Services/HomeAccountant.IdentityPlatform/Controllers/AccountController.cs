using AutoMapper;
using Domain.Dtos.IdentityPlatform;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace HomeAccountant.IdentityPlatform.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IMapper _mapper;

        public AccountController(UserManager<IdentityUser> userManager,
            IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        [HttpPut("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] UserPasswordChangeDto userPasswordChangeDto)
        {
            string? userId = GetUserId();

            if (userId is null)
                return BadRequest("Invalid payload");

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

            var user = await _userManager.FindByIdAsync(userId);

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

        [HttpGet("[action]/{email}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> GetUserId(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
            {
                return NotFound();
            }

            if (user.Email is null)
            {
                return BadRequest("The user didn't provide the email address");
            }

            return Ok(user.Id);
        }

        [HttpPost("[action]")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> GetUsers([Required] string[] userIds)
        {
            List<IdentityUser?> users = new List<IdentityUser?>();
            
            foreach (var userId in userIds)
            {
                var user = await _userManager.FindByIdAsync(userId);
                users.Add(user);
            }

            return Ok(_mapper.Map<UserModelDto[]>(users.Where(x => x is not null).ToArray()));
        }

        private string? GetUserId() => this.User.Claims.FirstOrDefault(x => x.Type == "UserId")?.Value;
    }
}
