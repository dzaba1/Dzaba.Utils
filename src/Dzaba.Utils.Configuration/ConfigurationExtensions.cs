using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Dzaba.Utils.Configuration;

/// <summary>
/// Provides extension methods for registering configuration settings and builders in the dependency injection container.
/// </summary>
public static class ConfigurationExtensions
{
    /// <summary>
    /// Registers a configuration section as a strongly-typed settings class in the dependency injection container.
    /// </summary>
    /// <typeparam name="T">The type of the settings class.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="sectionName">The name of the configuration section.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddSettings<T>(this IServiceCollection services, string sectionName)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentException.ThrowIfNullOrWhiteSpace(sectionName);

        services.AddTransient<T>(p =>
        {
            var config = p.GetRequiredService<IConfiguration>();
            var section = config.GetSection(sectionName);
            if (!section.Exists())
            {
                throw new InvalidOperationException($"Section '{sectionName}' is not defined.");
            }
            return section.Get<T>();
        });

        return services;
    }

    /// <summary>
    /// Registers a configuration instance in the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration instance.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddSingleton<IConfiguration>(configuration);
        return services;
    }

    /// <summary>
    /// Registers a configuration builder in the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configurationBuilder">The configuration builder function.</param>
    /// <param name="singleton">Indicates whether to register as a singleton.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddConfiguration(this IServiceCollection services, Func<IServiceProvider, IConfiguration> configurationBuilder,
        bool singleton = false)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configurationBuilder);

        if (singleton)
        {
            services.AddSingleton<IConfiguration>(configurationBuilder);
        }
        else
        {
            services.AddTransient<IConfiguration>(configurationBuilder);
        }
        return services;
    }

    /// <summary>
    /// Registers a JSON file-based configuration provider in the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="filePath">The path to the JSON file.</param>
    /// <param name="optional">Indicates whether the file is optional.</param>
    /// <param name="reloadOnChange">Indicates whether to reload the configuration when the file changes.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddJsonFileConfiguration(this IServiceCollection services, string filePath = "appsettings.json", bool optional = false, bool reloadOnChange = false)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        services.AddSingleton<IConfiguration>(p =>
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile(filePath, optional: optional, reloadOnChange: reloadOnChange);
            return builder.Build();
        });

        return services;
    }

    /// <summary>
    /// Registers an in-memory configuration provider in the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="initData">The initial configuration data.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddInMemoryConfiguration(this IServiceCollection services, IEnumerable<KeyValuePair<string,string>> initData)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(initData);

        services.AddSingleton<IConfiguration>(p =>
        {
            var builder = new ConfigurationBuilder()
                .AddInMemoryCollection(initData);
            return builder.Build();
        });

        return services;
    }

    /// <summary>
    /// Registers an in-memory configuration provider in the dependency injection container with initial data provided as params.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="initData">The initial configuration data.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddInMemoryConfiguration(this IServiceCollection services, params KeyValuePair<string,string>[] initData)
    {
        return services.AddInMemoryConfiguration((IEnumerable<KeyValuePair<string, string>>)initData);
    }
}
