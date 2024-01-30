namespace HomeAccountant.Core.Model
{
    public class TokenAuthenticationModel
    {
        public TokenAuthenticationModel(string token, string refreshToken)
        {
            Token = token;
            RefreshToken = refreshToken;
        }

        public string Token { get; private set; }
        public string RefreshToken { get; private set; }
    }
}
