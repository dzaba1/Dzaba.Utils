using System.Text.Json;
using Dzaba.Utils.AspNet.Hal;
using FluentAssertions;
using NUnit.Framework;

namespace Dzaba.Utils.AspNet.Tests.Hal;

[TestFixture]
public class HalEnvelopeTests
{
    private sealed record Item(int Id, string Name);

    private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web);

    private static IReadOnlyDictionary<string, HalLink> Links(string href) =>
        new Dictionary<string, HalLink> { ["self"] = new HalLink(href) };

    [Test]
    public void Wrap_WhenResource_ThenResourcePropertiesAndLinks()
    {
        var result = HalEnvelope.Wrap(new Item(1, "One"), Links("/items/1"), Options);

        result["id"]!.GetValue<int>().Should().Be(1);
        result["name"]!.GetValue<string>().Should().Be("One");
        result["_links"]!["self"]!["href"]!.GetValue<string>().Should().Be("/items/1");
    }

    [Test]
    public void Wrap_WhenLinkWithMethod_ThenMethodSerialized()
    {
        var links = new Dictionary<string, HalLink> { ["delete"] = new HalLink("/items/1", "DELETE") };

        var result = HalEnvelope.Wrap(new Item(1, "One"), links, Options);

        result["_links"]!["delete"]!["method"]!.GetValue<string>().Should().Be("DELETE");
    }

    [Test]
    public void Wrap_WhenNullResource_ThenException()
    {
        var act = () => HalEnvelope.Wrap<Item>(null!, Links("/x"), Options);

        act.Should().Throw<ArgumentNullException>().Where(e => e.ParamName == "resource");
    }

    [Test]
    public void Wrap_WhenNullLinks_ThenException()
    {
        var act = () => HalEnvelope.Wrap(new Item(1, "One"), null!, Options);

        act.Should().Throw<ArgumentNullException>().Where(e => e.ParamName == "links");
    }

    [Test]
    public void Wrap_WhenNullOptions_ThenException()
    {
        var act = () => HalEnvelope.Wrap(new Item(1, "One"), Links("/x"), null!);

        act.Should().Throw<ArgumentNullException>().Where(e => e.ParamName == "options");
    }

    [Test]
    public void WrapCollection_WhenNoItemLinks_ThenItemsEmbeddedWithoutLinks()
    {
        var items = new[] { new Item(1, "One"), new Item(2, "Two") };

        var result = HalEnvelope.WrapCollection("items", items, Links("/items"), Options);

        result["_links"]!["self"]!["href"]!.GetValue<string>().Should().Be("/items");
        var embedded = result["_embedded"]!["items"]!.AsArray();
        embedded.Should().HaveCount(2);
        embedded[0]!["id"]!.GetValue<int>().Should().Be(1);
        embedded[1]!["name"]!.GetValue<string>().Should().Be("Two");
        embedded[0]!.AsObject().ContainsKey("_links").Should().BeFalse();
    }

    [Test]
    public void WrapCollection_WhenItemLinks_ThenEachItemHasItsLinks()
    {
        var items = new[] { new Item(1, "One"), new Item(2, "Two") };

        var result = HalEnvelope.WrapCollection("items", items, Links("/items"), Options,
            item => Links($"/items/{item.Id}"));

        var embedded = result["_embedded"]!["items"]!.AsArray();
        embedded[0]!["_links"]!["self"]!["href"]!.GetValue<string>().Should().Be("/items/1");
        embedded[1]!["_links"]!["self"]!["href"]!.GetValue<string>().Should().Be("/items/2");
    }

    [Test]
    public void WrapCollection_WhenNoItems_ThenEmptyEmbeddedArray()
    {
        var result = HalEnvelope.WrapCollection("items", Array.Empty<Item>(), Links("/items"), Options);

        result["_embedded"]!["items"]!.AsArray().Should().BeEmpty();
    }

    [TestCase(null)]
    [TestCase("")]
    public void WrapCollection_WhenEmbeddedKeyNullOrEmpty_ThenException(string? key)
    {
        var act = () => HalEnvelope.WrapCollection(key!, Array.Empty<Item>(), Links("/items"), Options);

        act.Should().Throw<ArgumentException>().Where(e => e.ParamName == "embeddedKey");
    }

    [Test]
    public void WrapCollection_WhenNullItems_ThenException()
    {
        var act = () => HalEnvelope.WrapCollection<Item>("items", null!, Links("/items"), Options);

        act.Should().Throw<ArgumentNullException>().Where(e => e.ParamName == "items");
    }

    [Test]
    public void WrapCollection_WhenNullLinks_ThenException()
    {
        var act = () => HalEnvelope.WrapCollection("items", Array.Empty<Item>(), null!, Options);

        act.Should().Throw<ArgumentNullException>().Where(e => e.ParamName == "links");
    }

    [Test]
    public void WrapCollection_WhenNullOptions_ThenException()
    {
        var act = () => HalEnvelope.WrapCollection("items", Array.Empty<Item>(), Links("/items"), null!);

        act.Should().Throw<ArgumentNullException>().Where(e => e.ParamName == "options");
    }
}
