using Microsoft.Testing.Platform.Configurations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playground.RabbitMQ.Tests.Utils
{
    public static class IntegrationsUtility
    {
        public static void RegisterServices(IServiceCollection services, IConfiguration config)
        {
            var serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver(),
                TypeNameHandling = TypeNameHandling.None,
                Converters = new List<JsonConverter> { new StringEnumConverter() }
            };
            services.AddSingleton(config);
            services.AddLogging((x) =>
            {
                x.ClearProviders();
                x.AddConsole();
                x.SetMinimumLevel(LogLevel.Debug);
            });

            JsonConvert.DefaultSettings = () => serializerSettings;

            var builder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables();
            IConfiguration configuration = builder.Build();

            var conString = configuration.GetConnectionString("sykcros") ??
                throw new InvalidOperationException("Connection string 'sykcros' not found.");

            services.AddDbContext<KonamiContext>(options => options.UseOracle(conString));

            var playerIdPrefix = configuration.GetSection("Code").Value;
            services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile(new PatronToPlayerMap(playerIdPrefix ?? "UNK"));
                cfg.AddProfile<TransactionsProfile>();
            });


            services.UseKinectifyClient(configuration);
            services.AddScoped<PlayerClient>();
            services.AddScoped<GamingActivityClient>();

            // Player related services
            services.AddTransient<IPlayersService, PlayersService>();
            services.AddTransient<IPlayerEmailService, PlayerEmailService>();
            services.AddTransient<IPatronSignupService, PatronSignupService>();
            services.AddTransient<IPlayerAddressService, PlayerAddressService>();
            services.AddTransient<IPlayerOccupationsService, PlayerOccupationsService>();
            services.AddTransient<IPlayerPassportService, PlayerPassportService>();
            services.AddTransient<IPlayerPhoneService, PlayerPhoneService>();


            // Transaction related services
            services.AddTransient<IMessageSenderService, MessageSenderService>();

            // RabbitMQ configuration
            services.Configure<RabbitMQOptions>(config.GetSection("RabbitMQ"));
            services.AddSingleton<IConnectionFactory>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<RabbitMQOptions>>().Value;
                var endpoints = new System.Collections.Generic.List<AmqpTcpEndpoint> {
                    new AmqpTcpEndpoint("hostname"),
                    new AmqpTcpEndpoint("localhost")
                };

                return new ConnectionFactory
                {
                    HostName = options.HostName,
                    UserName = options.UserName,
                    Password = options.Password,
                    VirtualHost = options.VirtualHost
                };
            });

            var provider = services.BuildServiceProvider();
        }
    }
}
