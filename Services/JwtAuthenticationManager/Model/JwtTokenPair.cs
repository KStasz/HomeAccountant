namespace JwtAuthenticationManager.Model;

public class JwtTokenPair
{
    public JwtTokenPair(string token, string refreshToken)
    {
        Token = token;
        RefreshToken = refreshToken;
    }

    public string Token { get; private set; }
    public string RefreshToken { get; private set; }
}
