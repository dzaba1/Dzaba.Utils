using AutoFixture;
using Dzaba.Utils;
using Moq;

namespace Dzaba.TestUtils;

public static class TestExtensions
{
    public static Mock<T> FreezeMock<T>(this IFixture fixture)
        where T : class
    {
        Require.NotNull(fixture, nameof(fixture));

        return fixture.Freeze<Mock<T>>();
    }
}