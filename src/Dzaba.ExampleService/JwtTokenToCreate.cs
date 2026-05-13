using Dzaba.Utils.AspNet;
using System.ComponentModel.DataAnnotations;

namespace Dzaba.ExampleService;

public sealed class JwtTokenToCreate
{
    [Required]
    public JwtSettings Settings { get; set; }

    public TimeSpan Lifetime { get; set; } = TimeSpan.FromMinutes(30);

    public string[] Roles { get; set; }
}
