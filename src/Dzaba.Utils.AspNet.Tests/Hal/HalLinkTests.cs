using System.Text.Json;
using Dzaba.Utils.AspNet.Hal;
using FluentAssertions;
using NUnit.Framework;

namespace Dzaba.Utils.AspNet.Tests.Hal;

[TestFixture]
public class HalLinkTests
{
    [Test]
    public void Serialize_WhenMethodSet_ThenHrefAndMethodProperties()
    {
        var link = new HalLink("/api/items/1", "DELETE");

        var json = JsonSerializer.Serialize(link);

        json.Should().Be("""{"href":"/api/items/1","method":"DELETE"}""");
    }

    [Test]
    public void Constructor_WhenMethodOmitted_ThenMethodIsNull()
    {
        var link = new HalLink("/api/items");

        link.Href.Should().Be("/api/items");
        link.Method.Should().BeNull();
    }

    [Test]
    public void Equals_WhenSameValues_ThenEqual()
    {
        new HalLink("/a", "GET").Should().Be(new HalLink("/a", "GET"));
    }
}
