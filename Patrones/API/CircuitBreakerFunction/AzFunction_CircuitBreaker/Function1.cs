using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace AzFunction_CircuitBreaker
{
    public class Function1
    {
        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;

        public Function1(ILoggerFactory loggerFactory,
            IHttpClientFactory httpClientFactory)
        {
            _logger = loggerFactory.CreateLogger<Function1>();
            _httpClient = httpClientFactory.CreateClient("httpClientWithCircuitBreaker");
        }

        [Function("Function1")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req)
        {
            _logger.LogInformation($"C# HTTP trigger function processed a request at.{DateTime.Now }");

            try
            {
                var response = await _httpClient.GetAsync("Https://example.com/api/data");
                response.EnsureSuccessStatusCode();
                var data = await response.Content.ReadAsStringAsync();
                _logger.LogInformation($"Response: { data}");
                
            }
            catch(Exception ex)
            {
                _logger.LogError($"Error occurred: {ex.Message}");
            }
            return null;
        }
    }
}
