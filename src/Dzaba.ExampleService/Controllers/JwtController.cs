using Dzaba.AspNetUtils;
using Dzaba.AspNetUtils.ActionFilters;
using Dzaba.Utils.AspNet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Dzaba.ExampleService.Controllers;

[Route("jwt")]
[HandleErrors]
public class JwtController : ControllerBase
{
    private readonly ILogger<JwtController> logger;

    public JwtController(ILogger<JwtController> logger)
    {
        ArgumentNullException.ThrowIfNull(logger);

        this.logger = logger;
    }

    [HttpPost]
    public string Create([Required, FromBody]JwtTokenToCreate settings)
    {
        logger.LogInformation("Start building JWT token.");

        var builder = new JwtTokenBuilder()
            .WithAudience(settings.Settings.Audience)
            .WithIssuer(settings.Settings.Authority)
            .WithLifetime(settings.Lifetime)
            .WithSecurityKey(settings.Settings.IssuerSigningKey);

        if (settings.Roles != null)
        {
            foreach (var role in settings.Roles)
            {
                builder.WithClaim(ClaimTypes.Role, role);
            }
        }

        return builder.Build().EncodeToString();
    }

    [HttpGet("check")]
    [Authorize()]
    public bool CheckNoRoles()
    {
        logger.LogInformation("Checking no roles");

        return true;
    }

    [HttpGet("check/testRole")]
    [Authorize(Roles = "TestRole")]
    public bool CheckTestRole()
    {
        logger.LogInformation("Check TestRole");

        return true;
    }
}
