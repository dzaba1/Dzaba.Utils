namespace Dzaba.Utils.AspNet;

/// <summary>
/// Configuration settings for JWT tokens.
/// </summary>
public class JwtSettings
{
    /// <summary>
    /// The authorization server URL or authority endpoint.
    /// </summary>
    public string Authority { get; set; }

    /// <summary>
    /// The expected audience (client identifier) for JWT tokens.
    /// </summary>
    public string Audience { get; set; }

    /// <summary>
    /// Whether to require HTTPS when validating the token URL.
    /// </summary>
    public bool RequireHttps { get; set; }

    /// <summary>
    /// Whether to validate the audience claim in the JWT token.
    /// </summary>
    public bool ValidateAudience { get; set; } = true;

    /// <summary>
    /// Whether to validate the issuer claim in the JWT token.
    /// </summary>
    public bool ValidateIssuer { get; set; } = true;

    /// <summary>
    /// The signing key used to validate the JWT token.
    /// </summary>
    public string IssuerSigningKey { get; set; }
}
