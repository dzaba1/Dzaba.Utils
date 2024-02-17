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
        return new Fixture()
            .Customize(new AutoMoqCustomization());
    }
}
