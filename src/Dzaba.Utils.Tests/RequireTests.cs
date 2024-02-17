using Dzaba.Utils;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

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

    [Test]
    public void NotEmpty_WhenNullString_ThenException()
    {
        var name = "Test";

        this.Invoking(s => Require.NotEmpty(null, name))
            .Should().Throw<ArgumentNullException>().Where(e => e.ParamName == name);
    }

    [Test]
    public void NotEmpty_WhenEmptyString_ThenException()
    {
        var name = "Test";

        this.Invoking(s => Require.NotEmpty(string.Empty, name))
            .Should().Throw<ArgumentException>().Where(e => e.ParamName == name);
    }

    [Test]
    public void NotWhiteSpace_WhenNull_ThenException()
    {
        var name = "Test";

        this.Invoking(s => Require.NotWhiteSpace(null, name))
            .Should().Throw<ArgumentNullException>().Where(e => e.ParamName == name);
    }

    [TestCase("")]
    [TestCase("   ")]
    public void NotWhiteSpace_WhenWhiteSpace_ThenException(string value)
    {
        var name = "Test";

        this.Invoking(s => Require.NotWhiteSpace(value, name))
            .Should().Throw<ArgumentException>().Where(e => e.ParamName == name);
    }

    [Test]
    public void NotEmpty_WhenNullCollection_ThenException()
    {
        var name = "Test";
        IEnumerable<object> col = null;

        this.Invoking(s => Require.NotEmpty(col, name))
            .Should().Throw<ArgumentNullException>().Where(e => e.ParamName == name);
    }

    [Test]
    public void NotEmpty_WhenEmptyCollection_ThenException()
    {
        var name = "Test";
        IEnumerable<object> col = Enumerable.Empty<object>();

        this.Invoking(s => Require.NotEmpty(col, name))
            .Should().Throw<ArgumentException>().Where(e => e.ParamName == name);
    }
}