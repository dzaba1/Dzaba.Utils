using Serilog;
using Serilog.Core;
using System;

namespace Dzaba.TestUtils.Integration;

/// <summary>
/// Provides factory methods for creating and configuring Serilog logger instances for test scenarios.
/// </summary>
public static class TestLogging
{
    /// <summary>
    /// Creates and configures a Serilog logger instance for console output with thread ID enrichment and debug-level
    /// logging.
    /// </summary>
    /// <returns>A configured <see cref="Serilog.Core.Logger"/> instance that writes log events to the console with thread ID and
    /// source context information.</returns>
    public static Logger CreateSerilogLogger()
    {
        var config = new LoggerConfiguration();
        ConfigureSerilogLogger(config);
        return config.CreateLogger();
    }

    /// <summary>
    /// Configures the provided Serilog LoggerConfiguration to enrich logs with thread id, set minimum level to Debug,
    /// and write formatted output to the console.
    /// </summary>
    /// <param name="configuration">The LoggerConfiguration to configure.</param>
    public static void ConfigureSerilogLogger(LoggerConfiguration configuration)
    {
#if NET8_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(configuration);
#else
        if (configuration == null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }
#endif

        configuration.Enrich.WithThreadId()
            .MinimumLevel.Debug()
            .WriteTo.Console(outputTemplate: "{Timestamp:dd.MM.yyyy HH:mm:ss} [{SourceContext}] [{ThreadId}] [{Level:u3}] - {Message:lj}{NewLine}{Exception}");
    }
}
