using Dzaba.Utils.AspNet.Hal;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Dzaba.Utils.AspNet;

/// <summary>
/// Provides static extension methods for registering JWT authentication and the library's services.
/// </summary>
public static class Bootstrapper
{
    /// <summary>
    /// Adds JWT settings as a singleton service.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="settings">The JWT settings instance.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddJwtSettings(this IServiceCollection services, JwtSettings settings)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(settings);

        services.AddSingleton(settings);
        return services;
    }

    /// <summary>
    /// Adds JWT settings as a transient service using the provided factory.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="settings">A factory function to create JWT settings.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddJwtSettings(this IServiceCollection services, Func<IServiceProvider, JwtSettings> settings)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(settings);

        services.AddTransient(settings);
        return services;
    }

    /// <summary>
    /// Configures JWT authentication with the provided settings factory.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="optionsFactory">A factory function to create JWT settings.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services,
        Func<JwtSettings> optionsFactory)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(optionsFactory);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(jwtOptions =>
        {
            var options = optionsFactory();
            jwtOptions.Authority = options.Authority;
            jwtOptions.Audience = options.Audience;
            jwtOptions.RequireHttpsMetadata = options.RequireHttps;
            jwtOptions.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = options.ValidateAudience,
                ValidAudience = options.Audience,
                ValidateIssuer = options.ValidateIssuer
            };

            if (!string.IsNullOrWhiteSpace(options.IssuerSigningKey))
            {
                jwtOptions.TokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.IssuerSigningKey));
            }
        });

        return services;
    }

    /// <summary>
    /// Registers the services provided by this library, such as the HAL link factory.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddDzabaAspNetServices(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddTransient<ILinkFactory, LinkFactory>();

        return services;
    }
}

