using Dzaba.Utils.AspNet;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Dzaba.TestUtils.Integration.AspNet;

/// <summary>
/// Provides mocked JWT settings for integration tests.
/// </summary>
public class JwtMockedSettings
{
    /// <summary>
    /// The configured JWT settings.
    /// </summary>
    public JwtSettings Settings { get; } = new JwtSettings
    {
        Authority = "http://test",
        Audience = "home-security",
        IssuerSigningKey = "a-string-secret-at-least-256-bits-long",
        ValidateAudience = false,
        ValidateIssuer = false
    };

    /// <summary>
    /// Configures the service collection with mocked JWT settings.
    /// </summary>
    /// <param name="services">The service collection.</param>
    public void AddMockedJwtSettings(IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.RemoveAll<JwtSettings>();
        services.AddJwtSettings(Settings);
    }

    /// <summary>
    /// Gets a configured JWT token builder for test scenarios.
    /// </summary>
    /// <returns>A JwtTokenBuilder instance pre-configured with mocked settings.</returns>
    public JwtTokenBuilder GetTokenBuilder()
    {
        return new JwtTokenBuilder()
            .WithIssuer(Settings.Authority)
            .WithAudience(Settings.Audience)
            .WithSecurityKey(Settings.IssuerSigningKey)
            .WithLifetime(TimeSpan.FromMinutes(30));
    }
}

