using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Text;
using JwtAuthenticationManager;
using JwtAuthenticationManager.Config;
using JwtAuthenticationManager.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace HomeAccountant.IdentityPlatform;

[Route("api/[controller]")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IJwtTokenProvider _jwtTokenProvider;

    public AuthenticationController(UserManager<IdentityUser> userManager,
        IJwtTokenProvider jwtTokenProvider)
    {
        _userManager = userManager;
        _jwtTokenProvider = jwtTokenProvider;
    }

    [HttpPost]
    [Route("Register")]
    public async Task<IActionResult> Register([FromBody] UserRegistrationRequestDto userRegistrationRequestDto)
    {
        if (!ModelState.IsValid)
            return BadRequest();

        var userExists = await _userManager.FindByEmailAsync(userRegistrationRequestDto.Email);

        if (userExists is not null)
            return BadRequest(new AuthResult(errors: "User already exists"));

        var newUser = new IdentityUser()
        {
            UserName = userRegistrationRequestDto.UserName,
            Email = userRegistrationRequestDto.Email
        };

        var isUserCreated = await _userManager.CreateAsync(newUser, userRegistrationRequestDto.Password);

        if (!isUserCreated.Succeeded)
            return BadRequest(new AuthResult(isUserCreated.Errors.Select(x => x.Description)));

        var token = await _jwtTokenProvider.GenerateJwtToken(newUser);

        return Ok(new AuthResult(token: token.Token, refreshToken: token.RefreshToken));
    }

    [Route("Login")]
    [HttpPost]
    public async Task<IActionResult> Login([FromBody] UserLoginRequestDto userLoginRequestDto)
    {
        if (!ModelState.IsValid)
            return BadRequest();

        var existingUser = await _userManager.FindByEmailAsync(userLoginRequestDto.Email);

        if (existingUser is null)
            return BadRequest(new AuthResult(errors: "Password is not correct or user doesn't exist"));

        bool isCorrect = await _userManager.CheckPasswordAsync(existingUser, userLoginRequestDto.Password);

        if (!isCorrect)
            return BadRequest(new AuthResult(errors: "Password is not correct or user doesn't exist"));

        var token = await _jwtTokenProvider.GenerateJwtToken(existingUser);

        return Ok(new AuthResult(token: token.Token, refreshToken: token.RefreshToken));
    }

    [Route("RefreshToken")]
    [HttpPost]
    public async Task<IActionResult> Refreshtoken([FromBody] TokenRequestDto tokenRequest)
    {
        if (!ModelState.IsValid)
            return BadRequest(new AuthResult(errors: "Invalid parameters"));


        var result = await _jwtTokenProvider.VerifyAndGenerateToken(tokenRequest.Token, tokenRequest.RefreshToken);

        if (result == null)
            return BadRequest(new AuthResult(errors: "Invalid tokens"));


        return Ok(result);
    }
}
