using Dzaba.Utils;
using FluentAssertions;
using NUnit.Framework;
using System;

namespace Dzaba.CmdTemplate.Utils.Tests;

[TestFixture]
public class RequireTests
{
    [Test]
    public void NotNull_WhenNull_ThenException()
    {
        var name = "Test";

        this.Invoking(s => Require.NotNull(null, name))
            .Should().Throw<ArgumentNullException>().Where(e => e.ParamName == name);
    }

    [TestCase(null)]
    [TestCase("")]
    public void NotEmpty_WhenEmpty_ThenException(string value)
    {
        var name = "Test";

        this.Invoking(s => Require.NotEmpty(value, name))
            .Should().Throw<ArgumentNullException>().Where(e => e.ParamName == name);
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    public void NotWhiteSpace_WhenWhiteSpace_ThenException(string value)
    {
        var name = "Test";

        this.Invoking(s => Require.NotWhiteSpace(value, name))
            .Should().Throw<ArgumentNullException>().Where(e => e.ParamName == name);
    }
}