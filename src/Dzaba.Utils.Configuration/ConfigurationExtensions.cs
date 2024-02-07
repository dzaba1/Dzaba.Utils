using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Dzaba.Utils.Configuration;

public static class ConfigurationExtensions
{
    public static void RegisterDzabaSettings<T>(this IServiceCollection services, string sectionName)
        where T : class
    {
        Require.NotNull(services, nameof(services));
        Require.NotWhiteSpace(sectionName, nameof(sectionName));

        services.AddTransient<T>(p =>
        {
            var config = p.GetRequiredService<IConfiguration>();
            var section = config.GetSection(sectionName);
            return section.Get<T>();
        });
    }

    public static void RegisterDzabaConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        Require.NotNull(configuration, nameof(configuration));

        services.AddSingleton<IConfiguration>(configuration);
    }

    public static void RegisterDzabaConfiguration(this IServiceCollection services, Func<IServiceProvider, IConfiguration> configurationBuilder,
        bool singleton = false)
    {
        Require.NotNull(configurationBuilder, nameof(configurationBuilder));

        if (singleton)
        {
            services.AddSingleton<IConfiguration>(configurationBuilder);
        }
        else
        {
            services.AddTransient<IConfiguration>(configurationBuilder);
        }
    }
}
