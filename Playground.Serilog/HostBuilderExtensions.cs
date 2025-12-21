using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RW.CB.Common.Logger;
using Serilog;

namespace Playground.Serilog
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder AddSharedSerilog(this IHostBuilder hostBuilder, IConfiguration configuration)
        {
            if (configuration is null) throw new ArgumentNullException(nameof(configuration));

            // Inicializar Log.Logger antes de Build para que logs tempranos funcionen
            Log.Logger = SerilogConfigurator.CreateFromConfiguration(configuration);

            // Integrar con el host (requiere Serilog.Extensions.Hosting)
            hostBuilder.UseSerilog((hostingContext, services, loggerConfiguration) =>
            {
                loggerConfiguration
                    //.ReadFrom.Configuration(hostingContext.Configuration)
                    .ReadFrom.Services(services)
                    .Enrich.FromLogContext();
            });

            return hostBuilder;
        }

        // Nueva sobrecarga para IHostApplicationBuilder (usado por FunctionsApplicationBuilder)
        public static IHostApplicationBuilder AddSharedSerilog(this IHostApplicationBuilder hostAppBuilder, IConfiguration configuration)
        {
            if (configuration is null) throw new ArgumentNullException(nameof(configuration));

            // Inicializar Log.Logger antes de Build para que logs tempranos funcionen
            Log.Logger = SerilogConfigurator.CreateFromConfiguration(configuration);

            // Registrar Serilog como provider en el logging pipeline
            hostAppBuilder.Logging.AddSerilog();

            return hostAppBuilder;
        }

        // Nueva sobrecarga para IFunctionsWorkerApplicationBuilder (seguro para el builder de Azure Functions)
        public static IFunctionsWorkerApplicationBuilder AddSharedSerilog(this IFunctionsWorkerApplicationBuilder functionsBuilder, IConfiguration configuration)
        {
            if (configuration is null) throw new ArgumentNullException(nameof(configuration));

            // Inicializar Log.Logger antes de Build para que logs tempranos funcionen
            Log.Logger = SerilogConfigurator.CreateFromConfiguration(configuration);

            // El IFunctionsWorkerApplicationBuilder no expone Logging directamente, pero el objeto concreto
            // suele implementar también IHostApplicationBuilder. Intentamos hacer el registro si es posible.
            if (functionsBuilder is IHostApplicationBuilder hostApp)
            {
                hostApp.Services.AddSerilog();
            }

            return functionsBuilder;
        }
        // Nueva sobrecarga para IServiceCollection (registra Serilog en el pipeline de logging)
        public static IServiceCollection AddSharedSerilog(this IServiceCollection services, IConfiguration configuration)
        {
            if (services is null) throw new ArgumentNullException(nameof(services));
            if (configuration is null) throw new ArgumentNullException(nameof(configuration));

            // Inicializar Log.Logger antes de registrar el provider para que logs tempranos funcionen
            Log.Logger = SerilogConfigurator.CreateFromConfiguration(configuration);

            // Registrar Serilog como proveedor de logging
            services.AddLogging(loggingBuilder =>
            {
                //loggingBuilder.ClearProviders();
                loggingBuilder.AddSerilog(dispose: true);
            });

            return services;
        }
    }
}
