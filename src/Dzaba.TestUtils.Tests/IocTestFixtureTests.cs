using Dzaba.IntegrationTestUtils;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Dzaba.TestUtils.Tests;

[TestFixture]
public class IocTestFixtureTests : IocTestFixture
{
    protected override void RegisterServices(ServiceCollection services)
    {
        services.AddTransient<IMyService, MyService>();
    }

    [Test]
    public void Container_WhenCreated_AServiceCanBeResolved()
    {
        var service = Container.GetService<IMyService>();
        service.Should().NotBeNull();
        service.GetData().Should().Be("Hello, World!");
    }

    public interface IMyService
    {
        string GetData();
    }

    private class MyService : IMyService
    {
        public string GetData()
        {
            return "Hello, World!";
        }
    }
}
