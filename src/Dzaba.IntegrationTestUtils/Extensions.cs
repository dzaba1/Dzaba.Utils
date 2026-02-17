using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;

namespace Dzaba.IntegrationTestUtils;

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
    /// <returns>The original service collection, allowing for method chaining.</returns>
    public static IServiceCollection AddSerilogConsoleLogging(this IServiceCollection services)
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
        services.AddLogging(l => l.AddSerilog(logger, true));

        return services;
    }
}
