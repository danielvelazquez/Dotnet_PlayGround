using System;
using System.Threading;
using System.Threading.Tasks;

public sealed class InitialConfigurationHostedService : IHostedService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger _logger;

    public InitialConfigurationHostedService(IConfiguration configuration, ILogger logger)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.Information("Running initial configuration setup.");
        var ok = InitialConfigurationService.SetupInitialConfig(_configuration, _logger);
        if (!ok)
        {
            _logger.Fatal("Initial configuration setup failed. Aborting host startup.");
            // Provoca que el host falle al arrancar; puedes lanzar excepción o devolver Task.FromException
            return Task.FromException(new Exception("Initial configuration setup failed."));
        }

        _logger.Information("Initial configuration setup completed successfully.");
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
