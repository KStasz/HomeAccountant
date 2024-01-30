namespace HomeAccountant.IdentityPlatform;

public class AuthResult
{
    public AuthResult(string token, string refreshToken)
    {
        Token = token;
        RefreshToken = refreshToken;
    }

    public string Token { get; set; }
    public string RefreshToken { get; set; }
}
