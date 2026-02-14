using FluentAssertions;
using NUnit.Framework;

namespace Dzaba.TestUtils.Tests;

[TestFixture]
public class EmbeddedFileTests : TempTestFixture
{
    [Test]
    public void GetResourceStream_WhenResourceExists_ThenStreamIsReturned()
    {
        using var stream = EmbeddedFile.GetResourceStream(Path.Combine("Resources", "someText.txt"), GetType().Assembly);
        var content = new StreamReader(stream).ReadToEnd();
        content.Should().Be("Hello!");
    }

    [Test]
    public async Task ReadToEndAsync_WhenFileExists_ThenContent()
    {
        var content = await EmbeddedFile.ReadToEndAsync(Path.Combine("Resources", "someText.txt"), GetType().Assembly);
        content.Should().Be("Hello!");
    }

    [Test]
    public async Task CopyToAsync_WhenFileExists_ThenItIsCopied()
    {
        var target = Path.Combine(Temp, "copied.txt");
        await EmbeddedFile.CopyToAsync(Path.Combine("Resources", "someText.txt"), GetType().Assembly, target);

        var content = File.ReadAllText(target);
        content.Should().Be("Hello!");
    }
}
