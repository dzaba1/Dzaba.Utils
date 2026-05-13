using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Dzaba.Utils.AspNet;

/// <summary>
/// Fluent builder for creating JWT security tokens.
/// </summary>
public sealed class JwtTokenBuilder
{
    private SigningCredentials credentials;
    private string subject;
    private bool subjectSet;
    private string name;
    private bool nameSet;
    private string issuer;
    private string audience;
    private DateTime? expires;
    private DateTime? notBefore;
    private readonly List<Claim> customClaims = new List<Claim>();

    /// <summary>
    /// Configures the signing credentials used for the token.
    /// </summary>
    /// <param name="credentials">The signing credentials.</param>
    /// <returns>This builder instance.</returns>
    public JwtTokenBuilder WithCredentials(SigningCredentials credentials)
    {
        this.credentials = credentials;
        return this;
    }

    /// <summary>
    /// Configures the security key and optional algorithm for the token.
    /// </summary>
    /// <param name="key">The secret key for signing the token.</param>
    /// <param name="algorithm">The signing algorithm (default: HmacSha256).</param>
    /// <returns>This builder instance.</returns>
    public JwtTokenBuilder WithSecurityKey(string key, string algorithm = SecurityAlgorithms.HmacSha256)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, algorithm);
        return WithCredentials(credentials);
    }

    /// <summary>
    /// Sets the subject claim (typically the user ID or principal name).
    /// </summary>
    /// <param name="sub">The subject value.</param>
    /// <returns>This builder instance.</returns>
    public JwtTokenBuilder WithSubject(string sub)
    {
        this.subject = sub;
        subjectSet = true;
        return this;
    }

    /// <summary>
    /// Sets the name claim (typically the user's name).
    /// </summary>
    /// <param name="name">The name value.</param>
    /// <returns>This builder instance.</returns>
    public JwtTokenBuilder WithName(string name)
    {
        this.name = name;
        nameSet = true;
        return this;
    }

    /// <summary>
    /// Sets the issuer claim (who issued the token).
    /// </summary>
    /// <param name="issuer">The issuer value.</param>
    /// <returns>This builder instance.</returns>
    public JwtTokenBuilder WithIssuer(string issuer)
    {
        this.issuer = issuer;
        return this;
    }

    /// <summary>
    /// Sets the audience claim (intended recipients of the token).
    /// </summary>
    /// <param name="audience">The audience value.</param>
    /// <returns>This builder instance.</returns>
    public JwtTokenBuilder WithAudience(string audience)
    {
        this.audience = audience;
        return this;
    }

    /// <summary>
    /// Sets the absolute expiration time for the token.
    /// </summary>
    /// <param name="expires">The expiration DateTime.</param>
    /// <returns>This builder instance.</returns>
    public JwtTokenBuilder WithExpire(DateTime expires)
    {
        this.expires = expires;
        return this;
    }

    /// <summary>
    /// Sets the not-before time (when the token becomes valid).
    /// </summary>
    /// <param name="notBefore">The not-before DateTime.</param>
    /// <returns>This builder instance.</returns>
    public JwtTokenBuilder WithNotBefore(DateTime notBefore)
    {
        this.notBefore = notBefore;
        return this;
    }

    /// <summary>
    /// Sets the lifetime for the token (from now until expiration).
    /// </summary>
    /// <param name="lifetime">The time span for the token's lifetime.</param>
    /// <returns>This builder instance.</returns>
    public JwtTokenBuilder WithLifetime(TimeSpan lifetime)
    {
        this.expires = DateTime.Now.Add(lifetime);
        return this;
    }

    /// <summary>
    /// Adds a custom claim to the token.
    /// </summary>
    /// <param name="type">The claim type.</param>
    /// <param name="value">The claim value.</param>
    /// <returns>This builder instance.</returns>
    public JwtTokenBuilder WithClaim(string type, string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(type);

        var newClaim = new Claim(type, value);
        customClaims.Add(newClaim);
        return this;
    }

    /// <summary>
    /// Builds and returns the configured JWT security token.
    /// </summary>
    /// <returns>The JwtSecurityToken instance.</returns>
    public JwtSecurityToken Build()
    {
        var claims = new List<Claim>(customClaims);

        if (subjectSet)
        {
            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, subject));
        }

        if (nameSet)
        {
            claims.Add(new Claim(ClaimTypes.Name, name));
        }

        return new JwtSecurityToken(issuer, audience, claims, notBefore, expires, credentials);
    }
}

