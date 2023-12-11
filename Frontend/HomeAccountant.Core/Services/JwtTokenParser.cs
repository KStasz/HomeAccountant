using System.Security.Claims;
using System.Security.Principal;
using System.Text.Json;

namespace HomeAccountant.Core.Services
{
    public class JwtTokenParser : IJwtTokenParser
    {
        public const string EXPIRATION_DATE_CLAIM_TYPE = "exp";

        public IEnumerable<Claim>? ParseClaimsFromJwt(string token)
        {
            var payload = token.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
            var parsedKeyValuePairs = keyValuePairs?.Select(ParseClaims).ToList();

            if (keyValuePairs is null)
            {
                return null;
            }

            return parsedKeyValuePairs?.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString() ?? string.Empty));
        }

        public DateTime? GetTokenExpirationDate(string token)
        {
            var claims = ParseClaimsFromJwt(token);

            var utcExpiryDate = claims?.FirstOrDefault(x => x.Type == EXPIRATION_DATE_CLAIM_TYPE)?.Value;

            if (utcExpiryDate is null)
            {
                return null;
            }

            return UnixTimeStampToDateTime(long.Parse(utcExpiryDate));
        }

        private byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }

        private KeyValuePair<string, object> ParseClaims(KeyValuePair<string, object> pair, int arg2)
        {
            switch (pair.Key)
            {
                case "email":
                    return new KeyValuePair<string, object>(ClaimTypes.Name, pair.Value);
                case "UserId":
                    return new KeyValuePair<string, object>(ClaimTypes.NameIdentifier, pair.Value);
                default:
                    return new KeyValuePair<string, object>(pair.Key, pair.Value);
            }
        }

        private DateTime UnixTimeStampToDateTime(long utcExpiryDate)
        {
            return DateTimeOffset.FromUnixTimeSeconds(utcExpiryDate).DateTime;
        }
    }
}
