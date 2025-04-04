using Polly;
using Polly.CircuitBreaker;

namespace CircuitBreaker
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Configurar cliente HTTP
            HttpClient client = new HttpClient();

            // Configurar Circuit Breaker
            var circuitBreakerPolicy = Policy
                .Handle<HttpRequestException>() // Manejar excepciones específicas
                .CircuitBreakerAsync(
                    exceptionsAllowedBeforeBreaking: 3, // Número de fallos antes de abrir el circuito
                    durationOfBreak: TimeSpan.FromSeconds(10) // Tiempo que el circuito permanecerá abierto
                );

            // Simular llamadas al servicio con el Circuit Breaker
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    circuitBreakerPolicy.ExecuteAsync(async () =>
                    {
                        Console.WriteLine($"Intentando llamada {i + 1}...");

                        // Simular una llamada a un servicio externo
                        var response = await client.GetAsync("https://api.mockservice.com/data");
                        response.EnsureSuccessStatusCode();

                        Console.WriteLine("¡Llamada exitosa!");
                    }).Wait();
                }
                catch (BrokenCircuitException)
                {
                    Console.WriteLine("Circuito abierto: las solicitudes están bloqueadas temporalmente.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }

                // Esperar antes de la próxima solicitud
                System.Threading.Thread.Sleep(2000);
            }
        }
    }
}
