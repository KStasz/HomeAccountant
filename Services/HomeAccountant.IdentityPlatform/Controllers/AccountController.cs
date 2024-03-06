using AutoMapper;
using Domain.Controller;
using Domain.Dtos.IdentityPlatform;
using Domain.Model;
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
    public class AccountController : ServiceControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IMapper _mapper;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            UserManager<IdentityUser> userManager,
            IMapper mapper,
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpPut("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] UserPasswordChangeDto userPasswordChangeDto)
        {
            if (UserId is null)
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

            var user = await _userManager.FindByIdAsync(UserId);

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

        [HttpGet("[action]/{userId}")]
        public async Task<ActionResult<ServiceResponse<string?>>> GetEmailByUserId(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);

                if (user is null)
                    return NotFound(
                        new ServiceResponse(
                            new List<string>()
                            {
                                "The user with the specified Id could not be found"
                            }));

                if (user.Email is null)
                {
                    return BadRequest(
                        new ServiceResponse(
                            new List<string>()
                            {
                                "The user didn't provide the email address"
                            }));
                }

                return Ok(
                    new ServiceResponse<string>(user.Email));
            }
            catch (Exception ex)
            {
                _logger.LogError($"{Environment.NewLine}--> {ex.Message}{Environment.NewLine}");

                return BadRequest(
                    new ServiceResponse(
                        new List<string>()
                        {
                            "Unable to read user email"
                        }));
            }
        }

        [HttpPost("[action]")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<ServiceResponse<UserModelDto[]?>>> GetUsers([Required] string[] userIds)
        {
            try
            {
                List<IdentityUser?> users = new List<IdentityUser?>();

                foreach (var userId in userIds)
                {
                    var user = await _userManager.FindByIdAsync(userId);
                    users.Add(user);
                }

                return Ok(
                    new ServiceResponse<UserModelDto[]?>(
                    _mapper.Map<UserModelDto[]>(
                        users
                        .Where(x => x is not null)
                        .ToArray())));
            }
            catch (Exception ex)
            {
                _logger.LogError($"--> {ex.Message}");

                return BadRequest(new ServiceResponse("Reading users failed"));
            }
        }
    }
}
