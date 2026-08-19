using Playwrite.LectorDePagina.Servicios;

namespace Playwrite.LectorDePagina;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IPageReaderService _pageReaderService;

    public Worker(ILogger<Worker> logger,
        IPageReaderService pageReaderService)
    {
        _logger = logger;
        _pageReaderService = pageReaderService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await _pageReaderService.ReadBanxicoIndicators();
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            }
            await Task.Delay(3600, stoppingToken);
        }
    }
}
