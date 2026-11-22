using AutoFixture;
using FluentAssertions;
using NUnit.Framework;

namespace Dzaba.TestUtils.Tests;

[TestFixture]
public class TestFixtureTests
{
    [Test]
    public void Create_WhenNoOptions_ThenRecursiveTypeThrows()
    {
        var fixture = TestFixture.Create();

        fixture.Invoking(f => f.Create<Node>()).Should().Throw<ObjectCreationException>();
    }

    [Test]
    public void Create_WhenOmitOnRecursion_ThenRecursiveReferenceIsOmitted()
    {
        var fixture = TestFixture.Create(new TestFixtureOptions
        {
            RemoveThrowingRecursionBehavior = true,
            AddOmitOnRecursionBehavior = true
        });

        var result = fixture.Create<Node>();

        result.Name.Should().NotBeNullOrEmpty();
        result.Parent.Should().BeNull();
    }

    [Test]
    public void Create_WhenNoOptions_ThenMockMembersAreNotConfigured()
    {
        var fixture = TestFixture.Create();

        var result = fixture.Create<IHasName>();

        result.Name.Should().BeNull();
    }

    [Test]
    public void Create_WhenAutoMoqConfigureMembers_ThenMockMembersAreConfigured()
    {
        var fixture = TestFixture.Create(new TestFixtureOptions
        {
            AutoMoqConfigureMembers = true
        });

        var result = fixture.Create<IHasName>();

        result.Name.Should().NotBeNullOrEmpty();
    }

    [Test]
    public void Create_WhenOptionsNull_ThenThrows()
    {
        var act = () => TestFixture.Create(null);

        act.Should().Throw<ArgumentNullException>().WithParameterName("options");
    }

    public interface IHasName
    {
        string Name { get; }
    }

    public class Node
    {
        public string Name { get; set; }

        public Node Parent { get; set; }
    }
}
