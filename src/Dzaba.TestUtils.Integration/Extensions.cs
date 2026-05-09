using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System;

namespace Dzaba.TestUtils.Integration;

/// <summary>
/// Integration tests related extension methods.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Adds Serilog console logging to the specified service collection, enabling logging capabilities with a detailed
    /// output format.
    /// </summary>
    /// <param name="services">The service collection to which the Serilog console logging is added. This collection must not be null.</param>
    /// <param name="clearProviders">A boolean flag indicating whether to clear existing logging providers before adding Serilog. Defaults to true.</param>
    /// <returns>The original service collection, allowing for method chaining.</returns>
    public static IServiceCollection AddSerilogConsoleLogging(this IServiceCollection services, bool clearProviders = true)
    {
#if NET8_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(services);
#else
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }
#endif

        var logger = new LoggerConfiguration()
            .Enrich.WithThreadId()
            .MinimumLevel.Debug()
            .WriteTo.Console(outputTemplate: "{Timestamp:dd.MM.yyyy HH:mm:ss} [{SourceContext}] [{ThreadId}] [{Level:u3}] - {Message:lj}{NewLine}{Exception}")
            .CreateLogger();
        services.AddLogging(l =>
        {
            if (clearProviders)
            {
                l.ClearProviders();
            }
            l.AddSerilog(logger, true);
        });

        return services;
    }
}
