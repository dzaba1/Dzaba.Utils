using Dzaba.TestUtils.Integration.AspNet;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Dzaba.ExampleService.Tests.Integration;

[TestFixture]
public class JwtControllerTests : ControllerTestFixture<Program>
{
    protected override void OnConfigureServices(IServiceCollection services)
    {
        AddMockedJwtSettings(services);
    }

    private async Task<string> CreateTokenAsync(HttpClient client)
    {
        var body = new JwtTokenToCreate()
        {
            Settings = JwtSettings.Settings,
            Roles = ["TestRole"]
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, "jwt")
        {
            Content = JsonContent.Create(body),
        };

        return await InvokeAndAssertAsync(client, request).ConfigureAwait(false);
    }

    [Test]
    public async Task Create_WhenSettings_ThenTokenIsCreated()
    {
        var client = CreateClient();

        var result = await CreateTokenAsync(client);
        result.Should().NotBeNull();
    }

    [Test]
    public async Task CheckTestRole_WhenRoleCorrect_ThenOk()
    {
        var client = CreateClient();

        var token = await CreateTokenAsync(client);
        using var request = new HttpRequestMessage(HttpMethod.Get, "jwt/check/testRole");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var result = await InvokeAndAssertAsync(client, request).ConfigureAwait(false);
        result.Should().Be("true");
    }
}
