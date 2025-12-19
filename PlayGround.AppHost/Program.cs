var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Middlewares>("middlewares");

builder.Build().Run();
