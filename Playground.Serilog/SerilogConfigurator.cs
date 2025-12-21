using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;

namespace Playground.Serilog
{
    public static class SerilogConfigurator
    {
        public static ILogger CreateFromConfiguration(IConfiguration configuration)
        {
            var filePath = configuration["PropertyConfig:LoggerFilePath"];
            var level = configuration["PropertyConfig:LoggerLevel"];

            var cfg = new LoggerConfiguration()
                .MinimumLevel.Is(Enum.TryParse<LogEventLevel>(level, true, out var lvl) ? lvl : LogEventLevel.Information)
                .Enrich.FromLogContext()
                .Enrich.WithMachineName();
            //.Enrich.WithEnvironmentUserName()
            //.ReadFrom.Configuration(configuration);

            if (!string.IsNullOrWhiteSpace(filePath))
            {
                cfg.WriteTo.File(new RenderedCompactJsonFormatter(), filePath, rollingInterval: RollingInterval.Day);
            }
            return cfg.CreateLogger();
        }
    }
}
