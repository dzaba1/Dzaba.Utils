using System.Text.Json.Nodes;
using AutoFixture;
using Dzaba.TestUtils;
using Dzaba.Utils.AspNet.Hal;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace Dzaba.Utils.AspNet.Tests.Hal;

[TestFixture]
public class HalControllerTests : AutoFixtureTestFixture
{
    private sealed record Item(int Id, string Name);

    private sealed class TestController(ILinkFactory links, JsonOptions jsonOptions)
        : HalController(links, jsonOptions)
    {
        public HalLinksBuilder Builder() => CreateLinksBuilder();

        public OkObjectResult Single<T>(T resource, HalLinks links) => OkHal(resource, links);

        public OkObjectResult Collection<T>(string key, IEnumerable<T> items, HalLinks links, Func<T, HalLinks>? itemLinks = null) =>
            OkHal(key, items, links, itemLinks!);
    }

    private HttpContext httpContext = null!;
    private Mock<ILinkFactory> linkFactory = null!;

    [SetUp]
    public void SetUp()
    {
        httpContext = new DefaultHttpContext();
        linkFactory = Fixture.FreezeMock<ILinkFactory>();
    }

    private TestController CreateSut() => new(linkFactory.Object, new JsonOptions())
    {
        ControllerContext = new ControllerContext { HttpContext = httpContext },
    };

    [Test]
    public void CreateLinksBuilder_WhenLinkAdded_ThenUsesControllerHttpContextAndLinkFactory()
    {
        linkFactory
            .Setup(f => f.Build(httpContext, "Delete", "Users", null, HttpMethod.Delete))
            .Returns(new HalLink("/api/Users/1", "DELETE"));

        var links = CreateSut().Builder().AddDelete("Delete", "Users").Build();

        links.Delete.Should().Equal(new HalLink("/api/Users/1", "DELETE"));
    }

    [Test]
    public void OkHal_WhenResource_ThenOkWithWrappedResource()
    {
        var links = new HalLinks { Self = [new HalLink("/items/1")] };

        var result = CreateSut().Single(new Item(1, "One"), links);

        result.StatusCode.Should().Be(StatusCodes.Status200OK);
        var body = result.Value.Should().BeOfType<JsonObject>().Subject;
        body["id"]!.GetValue<int>().Should().Be(1);
        body["_links"]!["self"]!["href"]!.GetValue<string>().Should().Be("/items/1");
    }

    [Test]
    public void OkHal_WhenCollection_ThenOkWithEmbeddedItemsAndItemLinks()
    {
        var items = new[] { new Item(1, "One"), new Item(2, "Two") };
        var links = new HalLinks { Self = [new HalLink("/items")] };

        var result = CreateSut().Collection("items", items, links,
            item => new HalLinks { Self = [new HalLink($"/items/{item.Id}")] });

        result.StatusCode.Should().Be(StatusCodes.Status200OK);
        var body = result.Value.Should().BeOfType<JsonObject>().Subject;
        body["_links"]!["self"]!["href"]!.GetValue<string>().Should().Be("/items");
        var embedded = body["_embedded"]!["items"]!.AsArray();
        embedded.Should().HaveCount(2);
        embedded[1]!["_links"]!["self"]!["href"]!.GetValue<string>().Should().Be("/items/2");
    }

    [Test]
    public void OkHal_WhenCollectionWithoutItemLinks_ThenItemsHaveNoLinks()
    {
        var result = CreateSut().Collection("items", new[] { new Item(1, "One") }, new HalLinks());

        var body = result.Value.Should().BeOfType<JsonObject>().Subject;
        body["_embedded"]!["items"]!.AsArray()[0]!.AsObject().ContainsKey("_links").Should().BeFalse();
    }
}
