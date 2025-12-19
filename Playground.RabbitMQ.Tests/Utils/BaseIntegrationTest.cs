using Microsoft.Testing.Platform.Configurations;
using Microsoft.Testing.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playground.RabbitMQ.Tests.Utils
{
    public abstract class BaseIntegrationTest
    {
        protected static IConfiguration Configuration;
        protected static ILoggerFactory LogFactory;
        protected static ILogger<BaseIntegrationTest> _logger;
        protected static IServiceCollection _serviceCollection;
        protected IServiceProvider _provider;

        public static void BaseTestInit()
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                //.AddJsonFile($"appsettings.{environment}.json", true, true)
                .AddEnvironmentVariables();

            Configuration = config.Build();

            _serviceCollection = new ServiceCollection();

            _serviceCollection.AddSingleton(config);
            //_serviceCollection.AddSingleton(LogFactory);
            _serviceCollection.AddHttpClient();


            // _logger = LogFactory.CreateLogger<BaseIntegrationTest>();
        }
    }
}
