using AutoFixture;
using NUnit.Framework;

namespace Dzaba.TestUtils;

/// <summary>
/// Provides a base class for test fixtures that use AutoFixture to generate and customize test data for unit tests. With Moq customization.
/// </summary>
public abstract class AutoFixtureTestFixture : TempTestFixture
{
    /// <summary>
    /// Gets the fixture instance used to generate test data for the current context.
    /// </summary>
    protected IFixture Fixture { get; private set; }

    /// <summary>
    /// Initializes the test fixture before each test is executed.
    /// </summary>
    [SetUp]
    public void SetupFixture()
    {
        Fixture = AfterSetupFixture(TestFixture.Create());
    }

    /// <summary>
    /// Processes the specified fixture after the setup phase, allowing for additional customization or configuration.
    /// </summary>
    /// <param name="fixture">The fixture instance to be processed after setup.</param>
    /// <returns>The fixture instance after any post-setup processing has been applied.</returns>
    protected virtual IFixture AfterSetupFixture(IFixture fixture)
    {
        return fixture;
    }
}
