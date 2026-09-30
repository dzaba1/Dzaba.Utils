using AutoFixture;
using Dzaba.TestUtils;
using Dzaba.Utils.AspNet.Hal;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;

namespace Dzaba.Utils.AspNet.Tests.Hal;

[TestFixture]
public class HalLinksBuilderTests : AutoFixtureTestFixture
{
    private HttpContext httpContext = null!;
    private Mock<ILinkFactory> linkFactory = null!;

    [SetUp]
    public void SetUp()
    {
        httpContext = new DefaultHttpContext();
        Fixture.Inject(httpContext);
        linkFactory = Fixture.FreezeMock<ILinkFactory>();
        linkFactory
            .Setup(f => f.Build(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<object>(), It.IsAny<HttpMethod>()))
            .Returns((HttpContext _, string action, string controller, object _, HttpMethod method) =>
                new HalLink($"/{controller}/{action}", method?.Method));
    }

    private HalLinksBuilder CreateSut() => Fixture.Create<HalLinksBuilder>();

    [Test]
    public void Constructor_WhenNullHttpContext_ThenException()
    {
        var act = () => new HalLinksBuilder(null!, linkFactory.Object);

        act.Should().Throw<ArgumentNullException>().Where(e => e.ParamName == "httpContext");
    }

    [Test]
    public void Constructor_WhenNullLinkFactory_ThenException()
    {
        var act = () => new HalLinksBuilder(httpContext, null!);

        act.Should().Throw<ArgumentNullException>().Where(e => e.ParamName == "linkFactory");
    }

    [Test]
    public void AddDelete_WhenCalled_ThenDeleteLinkWithDeleteMethod()
    {
        var routeValues = new { id = 5 };

        var links = CreateSut().AddDelete("Delete", "Users", routeValues).Build();

        links.Delete.Should().Equal(new HalLink("/Users/Delete", "DELETE"));
        linkFactory.Verify(f => f.Build(httpContext, "Delete", "Users", routeValues, HttpMethod.Delete), Times.Once);
    }

    [Test]
    public void AddSelf_WhenCalled_ThenSelfLinkWithoutMethod()
    {
        var links = CreateSut().AddSelf("Get", "Users").Build();

        links.Self.Should().Equal(new HalLink("/Users/Get"));
        linkFactory.Verify(f => f.Build(httpContext, "Get", "Users", null, null), Times.Once);
    }

    [TestCase("get")]
    [TestCase("post")]
    [TestCase("put")]
    [TestCase("patch")]
    [TestCase("delete")]
    [TestCase("head")]
    [TestCase("options")]
    [TestCase("trace")]
    [TestCase("connect")]
    public void AddMethod_WhenCalled_ThenLinkUnderRelationWithMatchingMethod(string rel)
    {
        var sut = CreateSut();

        var links = (rel switch
        {
            "get" => sut.AddGet("A", "C"),
            "post" => sut.AddPost("A", "C"),
            "put" => sut.AddPut("A", "C"),
            "patch" => sut.AddPatch("A", "C"),
            "delete" => sut.AddDelete("A", "C"),
            "head" => sut.AddHead("A", "C"),
            "options" => sut.AddOptions("A", "C"),
            "trace" => sut.AddTrace("A", "C"),
            _ => sut.AddConnect("A", "C"),
        }).Build();

        links.Keys.Should().Equal(rel);
        links[rel].Should().Equal(new HalLink("/C/A", rel.ToUpperInvariant()));
    }

    [Test]
    public void AddDelete_WhenNameGiven_ThenLinkHasName()
    {
        var links = CreateSut().AddDelete("Delete", "Users", name: "byId").Build();

        links.Delete.Should().Equal(new HalLink("/Users/Delete", "DELETE", "byId"));
    }

    [Test]
    public void AddDelete_WhenCalledTwice_ThenBothLinksKept()
    {
        var links = CreateSut()
            .AddDelete("Delete", "Users", name: "byId")
            .AddDelete("DeleteByName", "Users", name: "byName")
            .Build();

        links.Delete.Should().HaveCount(2);
    }

    [Test]
    public void Add_WhenCustomRelation_ThenLinkUnderIt()
    {
        var links = CreateSut().Add("approve", "Approve", "Users", method: HttpMethod.Post).Build();

        links["approve"].Should().Equal(new HalLink("/Users/Approve", "POST"));
    }

    [TestCase(null)]
    [TestCase("")]
    public void Add_WhenRelationNullOrEmpty_ThenException(string? rel)
    {
        var act = () => CreateSut().Add(rel!, "A", "C");

        act.Should().Throw<ArgumentException>().Where(e => e.ParamName == "rel");
    }

    [Test]
    public void Build_WhenNothingAdded_ThenEmptyLinks()
    {
        CreateSut().Build().Should().BeEmpty();
    }
}
