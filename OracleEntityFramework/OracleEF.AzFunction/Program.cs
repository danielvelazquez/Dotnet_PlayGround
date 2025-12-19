using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


// Configuracion de Builder con estructura y comentarios para estudiarlos.
var builder = FunctionsApplication.CreateBuilder(args);

IConfiguration configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

// Registrar la configuración en el contenedor para que todo use la misma instancia.
builder.Services.AddSingleton<IConfiguration>(configuration);

// Configurar Functions web application
builder.ConfigureFunctionsWebApplication();

// Integración App Insights / Serilog (usa la configuración creada arriba)
builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

builder.Services.AddSharedSerilog(configuration);

// Asegurar que Serilog.Global esté disponible en DI (si la extensión no lo hace)
builder.Services.AddSingleton<Serilog.ILogger>(Log.Logger);

// Registrar el HostedService que realiza la configuración inicial.
// InitialConfigurationHostedService llamará a InitialConfigurationService.SetupInitialConfig(...) 
// y provocará fallo del arranque si la configuración inicial no es válida.
builder.Services.AddHostedService<InitialConfigurationHostedService>();

// Kinectify SDK configuration
builder.Services.UseKinectifyClient(configuration);
builder.Services.AddScoped<PlayerClient>();
builder.Services.AddScoped<GamingActivityClient>();

// DbContexts usando la misma IConfiguration registrada
var conString = configuration.GetConnectionString("sykcros") ??
     throw new InvalidOperationException("Connection string 'sykcros' not found.");
builder.Services.AddDbContext<KonamiContext>(options =>
    options.UseOracle(conString)
);

var talConString = configuration.GetConnectionString("sykcrosTAL") ??
     throw new InvalidOperationException("Connection string 'sykcrosTAL' not found.");
builder.Services.AddDbContext<KonamiTALContext>(options =>
    options.UseOracle(talConString)
);

// Servicios de aplicación
builder.Services.AddTransient<IKinectifyPlayerService, KinectifyPlayersService>();

// Player related services
builder.Services.AddTransient<IPlayersService, PlayersService>();
builder.Services.AddTransient<IPlayerEmailService, PlayerEmailService>();
builder.Services.AddTransient<IPatronSignupService, PatronSignupService>();
builder.Services.AddTransient<IPlayerAddressService, PlayerAddressService>();
builder.Services.AddTransient<IPlayerOccupationsService, PlayerOccupationsService>();
builder.Services.AddTransient<IPlayerPassportService, PlayerPassportService>();
builder.Services.AddTransient<IPlayerPhoneService, PlayerPhoneService>();
builder.Services.AddTransient<IPlayerCardService, PlayerCardService>();
builder.Services.AddTransient<PtnPhonesToPhones>();
builder.Services.AddTransient<IdentityDocumentsResolver>();
builder.Services.AddTransient<PtnAddressMaper>();
builder.Services.AddTransient<ILogFileManager, LogFileManager>();

var playerIdPrefix = configuration["PropertyConfig:Code"];
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile(new PatronToPlayerMap(playerIdPrefix ?? "UNK"));
});

builder.Build().Run();
