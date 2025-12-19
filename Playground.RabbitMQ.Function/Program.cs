using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.ConfigureFunctionsWebApplication();

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

// Transaction related services
builder.Services.AddTransient<IMessageSenderService, MessageSenderService>();


// RabbitMQ configuration
builder.Services.Configure<RabbitMQOptions>(builder.Configuration.GetSection("RabbitMQ"));
builder.Services.AddSingleton<IConnectionFactory>(sp =>
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
builder.Build().Run();
