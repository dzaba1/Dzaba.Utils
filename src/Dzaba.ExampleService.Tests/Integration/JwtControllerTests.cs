using Dzaba.TestUtils.Integration.AspNet;
using FluentAssertions;
using NUnit.Framework;
using System.Net.Http.Json;

namespace Dzaba.ExampleService.Tests.Integration;

[TestFixture]
public class JwtControllerTests : ControllerTestFixture<Program>
{
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

        var resp = await client.SendAsync(request).ConfigureAwait(false);
        return await AssertAsync(resp).ConfigureAwait(false);
    }

    [Test]
    public async Task Create_WhenSettings_ThenTokenIsCreated()
    {
        var client = CreateClient();

        var result = await CreateTokenAsync(client);
        result.Should().NotBeNull();
    }
}
