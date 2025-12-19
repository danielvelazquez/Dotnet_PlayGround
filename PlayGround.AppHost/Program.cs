var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Middlewares>("middlewares");

builder.AddAzureFunctionsProject<Projects.Playground_RabbitMQ_Function>("playground-rabbitmq-function");

builder.Build().Run();
