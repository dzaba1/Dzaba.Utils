namespace Dzaba.TestUtils;

/// <summary>
/// Options for <see cref="TestFixture.Create(TestFixtureOptions)"/>. The defaults match <see cref="TestFixture.Create()"/>.
/// </summary>
public class TestFixtureOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether mocks created by AutoMoq get their members set up to return values from the fixture.
    /// </summary>
    public bool AutoMoqConfigureMembers { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether AutoMoq creates delegates as mocks.
    /// </summary>
    public bool AutoMoqGenerateDelegates { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the default <c>ThrowingRecursionBehavior</c> is removed.
    /// Combine it with <see cref="AddOmitOnRecursionBehavior"/>; removing it alone makes recursive types overflow the stack.
    /// </summary>
    public bool RemoveThrowingRecursionBehavior { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether <c>OmitOnRecursionBehavior</c> is added, so recursive references are left unset instead of throwing.
    /// </summary>
    public bool AddOmitOnRecursionBehavior { get; set; }
}
