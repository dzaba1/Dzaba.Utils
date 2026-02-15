using AutoFixture;
using FluentAssertions;
using NUnit.Framework;

namespace Dzaba.TestUtils.Tests;

[TestFixture]
public class AutoFixtureTestFixtureTests : AutoFixtureTestFixture
{
    private MyService CreateSut()
    {
        return Fixture.Create<MyService>();
    }

    [Test]
    public void Create_WhenMockFrozen_ThenResult()
    {
        var dep = Fixture.FreezeMock<IMyDependency>();
        dep.Setup(x => x.Hello()).Returns(42);

        var sut = CreateSut();

        sut.Hello().Should().Be(42);
    }

    [Test]
    public void Temp_ItIsCreated()
    {
        Temp.Should().NotBeNullOrEmpty();
        Directory.Exists(Temp).Should().BeTrue();
    }

    public interface IMyDependency
    {
        int Hello();
    }

    private class MyService
    {
        private readonly IMyDependency dependency;

        public MyService(IMyDependency dependency)
        {
            this.dependency = dependency;
        }

        public int Hello()
        {
            return dependency.Hello();
        }
    }
}
