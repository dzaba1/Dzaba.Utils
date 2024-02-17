using AutoFixture;
using Dzaba.Utils;
using Moq;

namespace Dzaba.TestUtils;

/// <summary>
/// General extension methods.
/// </summary>
public static class TestExtensions
{
    /// <summary>
    /// Freezes <typeparamref name="T"/> service as <see cref="Mock{T}"/>.
    /// </summary>
    /// <typeparam name="T">Service type</typeparam>
    /// <param name="fixture"></param>
    /// <returns>The <see cref="Mock{T}"/> object</returns>
    public static Mock<T> FreezeMock<T>(this IFixture fixture)
        where T : class
    {
        Require.NotNull(fixture, nameof(fixture));

        return fixture.Freeze<Mock<T>>();
    }
}