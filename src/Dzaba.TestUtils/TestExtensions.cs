using AutoFixture;
using Moq;
using System;

#if !NET10_0_OR_GREATER
using Dzaba.Utils;
#endif

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
#if NET10_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(fixture);
#else
        Require.NotNull(fixture, nameof(fixture));
#endif

        return fixture.Freeze<Mock<T>>();
    }
}