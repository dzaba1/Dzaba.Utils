using AutoFixture;
using Dzaba.TestUtils;
using Dzaba.Utils.AspNet.Hal;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Moq;
using NUnit.Framework;

namespace Dzaba.Utils.AspNet.Tests.Hal;

[TestFixture]
public class LinkFactoryTests : AutoFixtureTestFixture
{
    private Mock<LinkGenerator> generator = null!;
    private HttpContext httpContext = null!;
    private RouteValueDictionary capturedValues;

    [SetUp]
    public void SetUp()
    {
        capturedValues = null;
        generator = Fixture.FreezeMock<LinkGenerator>();
        httpContext = new DefaultHttpContext();

        SetupPath("/api/items/5");
    }

    private void SetupPath(string path)
    {
        generator.Setup(g => g.GetPathByAddress<It.IsAnyType>(
                It.IsAny<HttpContext>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<RouteValueDictionary>(),
                It.IsAny<RouteValueDictionary>(),
                It.IsAny<PathString?>(),
                It.IsAny<FragmentString>(),
                It.IsAny<LinkOptions>()))
            .Callback(new InvocationAction(invocation => capturedValues = (RouteValueDictionary)invocation.Arguments[2]))
            .Returns(path);
    }

    private LinkFactory CreateSut() => Fixture.Create<LinkFactory>();

    [Test]
    public void Build_WhenPathGenerated_ThenLinkWithPathAndMethod()
    {
        var link = CreateSut().Build(httpContext, "Get", "Items", new { id = 5 }, HttpMethod.Get);

        link.Should().Be(new HalLink("/api/items/5", "GET"));
    }

    [Test]
    public void Build_WhenMethodOmitted_ThenMethodIsNull()
    {
        var link = CreateSut().Build(httpContext, "Get", "Items");

        link.Method.Should().BeNull();
    }

    [Test]
    public void Build_WhenCalled_ThenActionControllerAndRouteValuesPassedToGenerator()
    {
        CreateSut().Build(httpContext, "Get", "Items", new { id = 5 });

        capturedValues.Should().NotBeNull();
        capturedValues!["action"].Should().Be("Get");
        capturedValues["controller"].Should().Be("Items");
        capturedValues["id"].Should().Be(5);
    }

    [Test]
    public void Build_WhenNoPathGenerated_ThenException()
    {
        SetupPath(null);

        var act = () => CreateSut().Build(httpContext, "Get", "Items");

        act.Should().Throw<InvalidOperationException>().WithMessage("*Items.Get*");
    }

    [Test]
    public void Constructor_WhenNullGenerator_ThenException()
    {
        var act = () => new LinkFactory(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void Build_WhenNullHttpContext_ThenException()
    {
        var act = () => CreateSut().Build(null, "Get", "Items");

        act.Should().Throw<ArgumentNullException>().Where(e => e.ParamName == "httpContext");
    }

    [TestCase(null, "Items", "action")]
    [TestCase("", "Items", "action")]
    [TestCase("Get", null, "controller")]
    [TestCase("Get", "", "controller")]
    public void Build_WhenActionOrControllerNullOrEmpty_ThenException(string action, string controller, string paramName)
    {
        var act = () => CreateSut().Build(httpContext, action!, controller!);

        act.Should().Throw<ArgumentException>().Where(e => e.ParamName == paramName);
    }
}
