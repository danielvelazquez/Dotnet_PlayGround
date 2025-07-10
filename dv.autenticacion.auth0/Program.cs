using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace dv.autenticacion.auth0
{
    internal class Program
    {
        public static void RegisterServices(IServiceCollection services, IConfiguration config)
        {
            services.AddHttpClient("PlayersApi", (serviceProvider, client) =>
            {
                var config = serviceProvider.GetRequiredService<IConfiguration>();
                var section = config.GetSection("HttpClientSettings:PlayersApi");
                client.BaseAddress = new Uri(section.GetValue<string>("BaseAddress"));
                client.Timeout = TimeSpan.FromSeconds(section.GetValue<int>("TimeoutSeconds"));
            });
            services.AddHttpClient("TransactionsApi", (serviceProvider, client) =>
            {
                var config = serviceProvider.GetRequiredService<IConfiguration>();
                var section = config.GetSection("HttpClientSettings:TransactionsApi");
                client.BaseAddress = new Uri(section.GetValue<string>("BaseAddress"));
                client.Timeout = TimeSpan.FromSeconds(section.GetValue<int>("TimeoutSeconds"));
            });
        }
    }
}
