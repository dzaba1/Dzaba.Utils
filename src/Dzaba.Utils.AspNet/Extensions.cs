using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Dzaba.AspNetUtils;

/// <summary>
/// Provides extension helpers for ASP.NET Core HTTP context operations.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Gets the user's subject claim or name identifier claim from the HTTP context.
    /// </summary>
    /// <param name="context">The HTTP context containing the user claims.</param>
    /// <returns>The subject claim value if present; otherwise the name identifier claim value.</returns>
    public static string GetUserSubOrNameIdentifier(this HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        return context.User.GetUserSubOrNameIdentifier();
    }

    /// <summary>
    /// Gets the user's subject claim or name identifier claim from the HTTP context.
    /// </summary>
    /// <param name="user">The user claims to retrieve the subject claim from.</param>
    /// <returns>The subject claim value if present; otherwise the name identifier claim value.</returns>
    public static string GetUserSubOrNameIdentifier(this ClaimsPrincipal user)
    {
        ArgumentNullException.ThrowIfNull(user);

        var bySub = user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (!string.IsNullOrEmpty(bySub))
        {
            return bySub;
        }
        return user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }

    /// <summary>
    /// Encodes a JWT security token to a string.
    /// </summary>
    /// <param name="token">The JWT security token to encode.</param>
    /// <returns>The encoded JWT token as a string.</returns>
    public static string EncodeToString(this JwtSecurityToken token)
    {
        ArgumentNullException.ThrowIfNull(token);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
