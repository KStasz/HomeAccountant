namespace JwtAuthenticationManager.Config;

public class JwtConfig
{
    public required string Secret { get; set; }
    public required int ExpiresMinutes { get; set; }
}
