using System;
using System.Linq;
using AutoFixture;
using AutoFixture.AutoMoq;

namespace Dzaba.TestUtils;

/// <summary>
/// Static builders for <see cref="IFixture"/>.
/// </summary>
public static class TestFixture
{
    /// <summary>
    /// Creates a new <see cref="IFixture"/> with AutoMoq Customization.
    /// </summary>
    /// <returns><see cref="IFixture"/> object</returns>
    public static IFixture Create()
    {
        // A separate overload instead of an optional parameter, so assemblies compiled against the parameterless method keep binding to it.
        return Create(new TestFixtureOptions());
    }

    /// <summary>
    /// Creates a new <see cref="IFixture"/> with AutoMoq Customization configured by <paramref name="options"/>.
    /// </summary>
    /// <param name="options">Options for the fixture.</param>
    /// <returns><see cref="IFixture"/> object</returns>
    /// <exception cref="ArgumentNullException"><paramref name="options"/> is null.</exception>
    /// <example>
    /// <code>
    /// var fixture = TestFixture.Create(new TestFixtureOptions
    /// {
    ///     AutoMoqConfigureMembers = true,
    ///     RemoveThrowingRecursionBehavior = true,
    ///     AddOmitOnRecursionBehavior = true
    /// });
    /// </code>
    /// </example>
    public static IFixture Create(TestFixtureOptions options)
    {
        if (options is null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        var fixture = new Fixture()
            .Customize(new AutoMoqCustomization
            {
                ConfigureMembers = options.AutoMoqConfigureMembers,
                GenerateDelegates = options.AutoMoqGenerateDelegates
            });

        if (options.RemoveThrowingRecursionBehavior)
        {
            foreach (var behavior in fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToArray())
            {
                fixture.Behaviors.Remove(behavior);
            }
        }

        if (options.AddOmitOnRecursionBehavior)
        {
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        return fixture;
    }
}
