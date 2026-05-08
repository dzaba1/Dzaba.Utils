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
    public static string GetUserSubOrNameIdentifier(HttpContext context)
    {
        var bySub = context.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (!string.IsNullOrEmpty(bySub))
        {
            return bySub;
        }
        return context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }
}
