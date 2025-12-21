using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RW.CB.Common.Models;
using Serilog;
using System;

namespace RW.CB.Common.Services
{
    public static class InitialConfigurationService
    {
        private static IConfiguration? _configuration;
        private static Serilog.ILogger? _logger;
        private static readonly object _sync = new();

        /// <summary>
        /// Initialize the service with IConfiguration and a Serilog ILogger instance.
        /// </summary>
        public static void Initialize(IConfiguration configuration, Serilog.ILogger logger)
        {
            if (configuration is null) throw new ArgumentNullException(nameof(configuration));
            if (logger is null) throw new ArgumentNullException(nameof(logger));

            lock (_sync)
            {
                _configuration = configuration;
                _logger = logger;
            }
        }

        /// <summary>
        /// Compatibilidad: inicializa usando un Microsoft.Extensions.Logging.ILogger.
        /// Este overload usa Serilog.Log.Logger como logger por defecto y escribe una advertencia
        /// para indicar que se debería pasar un Serilog.ILogger (uso preferente).
        /// </summary>
        public static void Initialize(IConfiguration configuration, Microsoft.Extensions.Logging.ILogger msLogger)
        {
            if (configuration is null) throw new ArgumentNullException(nameof(configuration));
            if (msLogger is null) throw new ArgumentNullException(nameof(msLogger));

            // Usar el logger global de Serilog configurado en la aplicación como fallback.
            // Si tu extensión registra Serilog.ILogger en DI, llama a Initialize(configuration, serilogLogger) en su lugar.
            lock (_sync)
            {
                _configuration = configuration;
                _logger = Log.Logger;
                _logger.Warning("InitialConfigurationService inicializado con un ILogger de Microsoft. Usar Serilog.ILogger es recomendado.");
            }
        }

        /// <summary>
        /// Indicates whether the service has been initialized.
        /// </summary>
        public static bool IsInitialized
        {
            get
            {
                lock (_sync)
                {
                    return _configuration != null && _logger != null;
                }
            }
        }

        /// <summary>
        /// Configures the initial settings using the previously initialized IConfiguration and Serilog ILogger.
        /// </summary>
        public static bool SetupInitialConfig()
        {
            if (!IsInitialized)
                throw new InvalidOperationException("InitialConfigurationService no está inicializado. Llama a Initialize(configuration, logger) primero o usa SetupInitialConfig(configuration, logger).");

            // _configuration and _logger are non-null by IsInitialized
            return SetupInitialConfig(_configuration!, _logger!);
        }

        /// <summary>
        /// Version that allows explicitly passing IConfiguration and a Serilog ILogger without relying on static state.
        /// </summary>
        public static bool SetupInitialConfig(IConfiguration configuration, Serilog.ILogger logger)
        {
            if (configuration is null) throw new ArgumentNullException(nameof(configuration));
            if (logger is null) throw new ArgumentNullException(nameof(logger));

            try
            {
                var propertyCode = configuration.GetValue<string>("PropertyConfig:Code");
                var totalToSend = configuration.GetValue<int?>("PropertyConfig:TestTotal") ?? 10000;
                var batchSize = configuration.GetValue<int?>("PropertyConfig:BatchSize") ?? 500;
                var maxParallelism = configuration.GetValue<int?>("PropertyConfig:MaxParallelism") ?? 20;
                var gamingHoursStart = configuration.GetValue<int?>("PropertyConfig:GamingHoursStart") ?? 6;

                ConfigurationValues.PropertyCode = !string.IsNullOrEmpty(propertyCode) ? propertyCode : string.Empty;
                ConfigurationValues.TestTotal = totalToSend;
                ConfigurationValues.BatchSize = batchSize;
                ConfigurationValues.MaxParallelism = maxParallelism;
                ConfigurationValues.GamingHoursStart = gamingHoursStart;

                if (!string.IsNullOrEmpty(propertyCode))
                    return true;

                logger.Error("Property Code configuration is missing.");
                return false;
            }
            catch (Exception ex)
            {
                // Loguear excepción completa para depuración usando Serilog
                logger.Error(ex, "Error al cargar la configuración inicial.");
                return false;
            }
        }

        /// <summary>
        /// Compatibilidad: permite llamar con un Microsoft.Extensions.Logging.ILogger.
        /// Convertimos al logger global de Serilog para la implementación actual.
        /// </summary>
        public static bool SetupInitialConfig(IConfiguration configuration, Microsoft.Extensions.Logging.ILogger msLogger)
        {
            if (configuration is null) throw new ArgumentNullException(nameof(configuration));
            if (msLogger is null) throw new ArgumentNullException(nameof(msLogger));

            var serilogLogger = Log.Logger;
            serilogLogger.Warning("SetupInitialConfig llamado con ILogger de Microsoft. Pasar Serilog.ILogger es recomendado.");
            return SetupInitialConfig(configuration, serilogLogger);
        }
    }
}
