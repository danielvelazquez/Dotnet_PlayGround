var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Middlewares>("middlewares");

builder.AddProject<Projects.Playwrite_LectorDePagina>("playwrite-lectordepagina");

builder.Build().Run();
