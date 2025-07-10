using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using dv.autenticacion.auth0.Modelos;

namespace dv.autenticacion.auth0
{
    public class HttpClientBase : IHttpClientBase
    {
        private IAuthService _authService;
        private ILogger<HttpClientBase> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private IConfiguration _configuration;

        public HttpClientBase(
            IAuthService authService,
            ILogger<HttpClientBase> logger,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        /// <summary>
        /// API Health Check 
        /// </summary>
        /// <returns></returns>
        public async Task<HttpResponse> APIHealthCheck()
        {
            var healthCheckEndPoint = _configuration["PlayerHelthCheck:EndPoint"];

            if (string.IsNullOrEmpty(healthCheckEndPoint))
            {
                throw new InvalidOperationException("Auth0 configuration is not properly set.");
            }

            var client = _httpClientFactory.CreateClient();
            var token = await _authService.GetAuth0TokenAsync();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            try
            {
                var response = await client.GetAsync(healthCheckEndPoint);
                return new HttpResponse
                {
                    Success = response.IsSuccessStatusCode,
                    StatusCode = response.StatusCode,
                    ErrorMessage = response.IsSuccessStatusCode == false ? "No Success" : $"Response Code: {response.StatusCode}"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Players Health Check.");
                return new HttpResponse
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public async Task<HttpResponse> PostRequest(string apiEndpoint, string jsonContent)
        {
            if (string.IsNullOrEmpty(apiEndpoint) || string.IsNullOrEmpty(jsonContent))
            {
                throw new ArgumentException("API endpoint and JSON content must be provided.");
            }
            var client = _httpClientFactory.CreateClient();
            var token = await _authService.GetAuth0TokenAsync();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            try
            {
                var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
                var response = await client.PostAsync(apiEndpoint, content);
                return new HttpResponse
                {
                    Success = response.IsSuccessStatusCode,
                    StatusCode = response.StatusCode,
                    ErrorMessage = response.IsSuccessStatusCode ? null : $"Response Code: {response.StatusCode}"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Players Post Request.");
                return new HttpResponse
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }
    }
}
