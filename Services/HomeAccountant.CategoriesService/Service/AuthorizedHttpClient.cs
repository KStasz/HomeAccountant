using Microsoft.Identity.Client;
using System.Net.Http.Headers;

namespace HomeAccountant.CategoriesService.Service
{
    public class AuthorizedHttpClient
    {
        private readonly HttpClient _client;
        private readonly IServiceProvider _serviceProvider;

        public AuthorizedHttpClient(HttpClient client,
            IServiceProvider serviceProvider)
        {
            _client = client;
            _serviceProvider = serviceProvider;
            ConfigureHttpClient();
        }

        private void ConfigureHttpClient()
        {
            var httpContext = _serviceProvider.GetRequiredService<IHttpContextAccessor>();
            var authHader = httpContext.HttpContext?.Request.Headers.Authorization.ToString() ?? string.Empty;
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                authHader.Replace("Bearer ", string.Empty));
        }

        public Task<HttpResponseMessage> GetAsync(string? requestUrl)
        {
            return _client.GetAsync(requestUrl);
        }

        public Task<HttpResponseMessage> DeleteAsync(string? requestUrl)
        {
            return _client.DeleteAsync(requestUrl);
        }
    }
}
