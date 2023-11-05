namespace HomeAccountant.IdentityPlatform;

public class AuthResult
{
    public AuthResult(string token, string refreshToken)
    {
        Token = token;
        RefreshToken = refreshToken;
        Result = true;
    }

    public AuthResult(params string[] errors)
    {
        Result = false;
        Errors = new List<string>(errors);
    }

    public AuthResult(IEnumerable<string> errors)
    {
        Result = false;
        Errors = new List<string>(errors);
    }

    public string? Token { get; set; }
    public string? RefreshToken { get; set; }
    public bool Result { get; set; }
    public List<string>? Errors { get; set; }
}
