namespace JwtAuthenticationManager
{
    public interface IAuthorizationTokenProvider
    {
        string GetToken();
    }
}