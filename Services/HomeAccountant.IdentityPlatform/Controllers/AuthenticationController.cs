using Domain.Dtos.IdentityPlatform;
using Domain.Model;
using JwtAuthenticationManager;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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
    public async Task<ActionResult<ServiceResponse<AuthResult>>> Register([FromBody] UserRegistrationRequestDto userRegistrationRequestDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(
                new ServiceResponse<AuthResult>(
                    new List<string>()
                    {
                        "Przesłano niepoprawne dane"
                    }));

        var userExists = await _userManager.FindByEmailAsync(userRegistrationRequestDto.Email);

        if (userExists is not null)
            return BadRequest(
                new ServiceResponse<AuthResult>(
                    new List<string>()
                    {
                        "Użytkownik o podanym adresie Email już istnieje"
                    }));

        var newUser = new IdentityUser()
        {
            UserName = userRegistrationRequestDto.UserName,
            Email = userRegistrationRequestDto.Email
        };

        var isUserCreated = await _userManager.CreateAsync(newUser, userRegistrationRequestDto.Password);

        if (!isUserCreated.Succeeded)
            return BadRequest(
                new ServiceResponse<AuthResult>(
                    new List<string>(isUserCreated.Errors.Select(x => x.Description))));

        var token = await _jwtTokenProvider.GenerateJwtToken(newUser);

        return Ok(new ServiceResponse<AuthResult>(
            new AuthResult(token: token.Token, refreshToken: token.RefreshToken)));
    }

    [Route("Login")]
    [HttpPost]
    public async Task<ActionResult<ServiceResponse<AuthResult>>> Login([FromBody] UserLoginRequestDto userLoginRequestDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(
                new ServiceResponse<AuthResult>(
                    new List<string>()
                    {
                        "Przesłano niepoprawne dane"
                    }));

        var existingUser = await _userManager.FindByEmailAsync(userLoginRequestDto.Email);

        if (existingUser is null)
            return BadRequest(
                new ServiceResponse<AuthResult>(
                    new List<string>()
                    {
                        "Hasło jest niepoprawne lub użytkownik nie istnieje"
                    }));

        bool isCorrect = await _userManager.CheckPasswordAsync(existingUser, userLoginRequestDto.Password);

        if (!isCorrect)
            return BadRequest(
                new ServiceResponse<AuthResult>(
                    new List<string>()
                    {
                        "Hasło jest niepoprawne lub użytkownik nie istnieje"
                    }));

        var token = await _jwtTokenProvider.GenerateJwtToken(existingUser);

        return Ok(
            new ServiceResponse<AuthResult>(
                new AuthResult(token: token.Token, refreshToken: token.RefreshToken)));
    }

    [Route("RefreshToken")]
    [HttpPost]
    public async Task<ActionResult<ServiceResponse<AuthResult>>> Refreshtoken([FromBody] TokenRequestDto tokenRequest)
    {
        if (!ModelState.IsValid)
            return BadRequest(
                new ServiceResponse<AuthResult>(
                    new List<string>()
                    {
                        "Przesłano niepoprawne dane"
                    }));

        var result = await _jwtTokenProvider.VerifyAndGenerateToken(tokenRequest.Token, tokenRequest.RefreshToken);

        if (result == null)
            return BadRequest(
                new ServiceResponse<AuthResult>(
                    new List<string>()
                    {
                        "Niepoprawne tokeny"
                    }));

        return Ok(result);
    }
}
