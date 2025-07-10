using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;

namespace dv.autenticacion.auth0
{
    public class AuthService : IAuthService
    {
        private IConfiguration _configuration;
        private ILogger<AuthService> _logger;
        private IHttpClientFactory _httpClientFactory;
       
        public AuthService(
            ILogger<AuthService> logger,
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// Obtains an Auth0 token for API access.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<string> GetAuth0TokenAsync()
        {
            var domain = _configuration["Auth0:Domain"];
            var clientId = _configuration["Auth0:ClientId"];
            var clientSecret = _configuration["Auth0:ClientSecret"];
            var audience = _configuration["Auth0:Audience"];

            if (string.IsNullOrEmpty(domain) || string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret) || string.IsNullOrEmpty(audience))
            {
                _logger.LogError("Auth0 configuration is not properly set. Domain: {Domain}, ClientId: {ClientId}, Audience: {Audience}", domain, clientId, audience);
                throw new InvalidOperationException("Auth0 configuration is not properly set.");
            }

            try
            {
                var client = _httpClientFactory.CreateClient();
                var request = new HttpRequestMessage(HttpMethod.Post, domain);

                request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    {"client_id", clientId},
                    {"client_secret", clientSecret},
                    {"audience", audience},
                    {"grant_type", "client_credentials"}
                });

                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                var token = JsonDocument.Parse(json).RootElement.GetProperty("access_token").GetString();
                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogError("Failed to retrieve access token from Auth0 response.");
                    throw new InvalidOperationException("Failed to retrieve access token from Auth0 response.");
                }
                return token;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obtaining Auth0 token.");
                throw new InvalidOperationException("Failed to obtain Auth0 token.", ex);
            }
        }

        
    }
}
