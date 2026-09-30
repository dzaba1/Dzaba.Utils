using System.Text.Json;
using System.Text.Json.Nodes;
using Dzaba.Utils.AspNet.Hal;
using FluentAssertions;
using NUnit.Framework;

namespace Dzaba.Utils.AspNet.Tests.Hal;

[TestFixture]
public class HalLinksTests
{
    private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web);

    [Test]
    public void Properties_WhenSet_ThenStoredUnderLowercaseRelation()
    {
        var link = new HalLink("/x");

        var links = new HalLinks
        {
            Self = [link],
            Get = [link],
            Post = [link],
            Put = [link],
            Patch = [link],
            Delete = [link],
            Head = [link],
            Options = [link],
            Trace = [link],
            Connect = [link],
        };

        links.Keys.Should().BeEquivalentTo(
            "self", "get", "post", "put", "patch", "delete", "head", "options", "trace", "connect");
        links["delete"].Should().Equal(link);
    }

    [Test]
    public void Property_WhenNotSet_ThenEmptyList()
    {
        new HalLinks().Self.Should().BeEmpty();
    }

    [Test]
    public void Property_WhenReadOnly_ThenRelationNotVisible()
    {
        var links = new HalLinks();

        _ = links.Self;

        links.ContainsKey("self").Should().BeFalse();
        links.Keys.Should().BeEmpty();
        links.Count.Should().Be(0);
        links.Should().BeEmpty();
    }

    [Test]
    public void Property_WhenSetToNull_ThenRelationRemoved()
    {
        var links = new HalLinks { Self = [new HalLink("/x")] };

        links.Self = null;

        links.ContainsKey("self").Should().BeFalse();
        links.Count.Should().Be(0);
    }

    [Test]
    public void Property_WhenListModifiedInPlace_ThenLinksStored()
    {
        var links = new HalLinks();

        links.Delete.Add(new HalLink("/x/1", "DELETE"));
        links.Delete.Add(new HalLink("/x/one", "DELETE"));

        links.TryGetValue("delete", out var all).Should().BeTrue();
        all.Should().Equal(new HalLink("/x/1", "DELETE"), new HalLink("/x/one", "DELETE"));
    }

    [Test]
    public void Property_WhenNestedCollectionInitializer_ThenLinksStored()
    {
        var links = new HalLinks { Delete = { new HalLink("/x/1"), new HalLink("/x/2") } };

        links.Delete.Should().HaveCount(2);
    }

    [Test]
    public void Property_WhenSetToEmptyList_ThenRelationNotVisible()
    {
        var links = new HalLinks { Self = [] };

        links.ContainsKey("self").Should().BeFalse();
        links.Count.Should().Be(0);
    }

    [Test]
    public void Indexer_WhenCustomRelation_ThenStored()
    {
        var links = new HalLinks { ["approve"] = [new HalLink("/x/approve", "POST")] };

        links["approve"].Should().Equal(new HalLink("/x/approve", "POST"));
        links.TryGetValue("approve", out var all).Should().BeTrue();
        all.Should().Equal(new HalLink("/x/approve", "POST"));
    }

    [Test]
    public void Indexer_WhenSetOnRelationWithManyLinks_ThenReplacesThemAll()
    {
        var links = new HalLinks { { "item", new HalLink("/a") }, { "item", new HalLink("/b") } };

        links["item"] = [new HalLink("/c")];

        links["item"].Should().Equal(new HalLink("/c"));
    }

    [Test]
    public void DictionaryIndexer_WhenNoLinks_ThenKeyNotFound()
    {
        IReadOnlyDictionary<string, IReadOnlyList<HalLink>> links = new HalLinks();

        var act = () => links["self"];

        act.Should().Throw<KeyNotFoundException>();
    }

    [Test]
    public void Add_WhenCustomRelation_ThenStored()
    {
        var links = new HalLinks { { "approve", new HalLink("/x/approve", "POST") } };

        links["approve"].Should().ContainSingle().Which.Href.Should().Be("/x/approve");
    }

    [Test]
    public void Add_WhenRelationAlreadySet_ThenLinkAppended()
    {
        var links = new HalLinks { Delete = [new HalLink("/x/1", "DELETE")] };

        links.Add("delete", new HalLink("/x/one", "DELETE"));

        links.Delete.Should().Equal(new HalLink("/x/1", "DELETE"), new HalLink("/x/one", "DELETE"));
    }

    [Test]
    public void Add_WhenNullLink_ThenException()
    {
        var act = () => new HalLinks().Add("self", null!);

        act.Should().Throw<ArgumentNullException>().Where(e => e.ParamName == "link");
    }

    [TestCase(null)]
    [TestCase("")]
    public void Indexer_WhenRelationNullOrEmpty_ThenException(string? rel)
    {
        var act = () => new HalLinks()[rel!];

        act.Should().Throw<ArgumentException>().Where(e => e.ParamName == "rel");
    }

    [Test]
    public void Serialize_WhenLinks_ThenObjectKeyedByRelation()
    {
        var links = new HalLinks
        {
            Self = [new HalLink("/x")],
            Delete = [new HalLink("/x", "DELETE")],
            ["approve"] = [new HalLink("/x/approve", "POST")],
        };

        var json = JsonSerializer.SerializeToNode(links, Options)!.AsObject();

        json.Select(p => p.Key).Should().Equal("self", "delete", "approve");
        json["delete"]!["method"]!.GetValue<string>().Should().Be("DELETE");
        json["approve"]!["href"]!.GetValue<string>().Should().Be("/x/approve");
    }

    [Test]
    public void Serialize_WhenRelationHasManyLinks_ThenArrayOfLinks()
    {
        var links = new HalLinks
        {
            { "self", new HalLink("/x") },
            { "delete", new HalLink("/x/1", "DELETE", "byId") },
            { "delete", new HalLink("/x/one", "DELETE", "byName") },
        };

        var json = JsonSerializer.SerializeToNode(links, Options)!.AsObject();

        json["self"].Should().BeAssignableTo<JsonObject>();
        var delete = json["delete"]!.AsArray();
        delete.Should().HaveCount(2);
        delete[0]!["href"]!.GetValue<string>().Should().Be("/x/1");
        delete[0]!["name"]!.GetValue<string>().Should().Be("byId");
        delete[1]!["href"]!.GetValue<string>().Should().Be("/x/one");
        delete[1]!["name"]!.GetValue<string>().Should().Be("byName");
    }

    [Test]
    public void Serialize_WhenRelationEmpty_ThenOmitted()
    {
        var links = new HalLinks { Self = [new HalLink("/x")] };
        _ = links.Delete;

        var json = JsonSerializer.SerializeToNode(links, Options)!.AsObject();

        json.Select(p => p.Key).Should().Equal("self");
    }

    [Test]
    public void Serialize_WhenLinkHasNoName_ThenNameOmitted()
    {
        var links = new HalLinks { Self = [new HalLink("/x")] };

        var json = JsonSerializer.SerializeToNode(links, Options)!.AsObject();

        json["self"]!.AsObject().ContainsKey("name").Should().BeFalse();
    }
}
