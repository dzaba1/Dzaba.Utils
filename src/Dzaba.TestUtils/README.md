# Dzaba.TestUtils

Utilities for unit and integration tests.

## Usage

```csharp
[TestFixture]
public class MyServiceTests
{
	private IFixutre fixture;

	[SetUp]
	public void Setup()
	{
		fixture = TestFixture.Create();
	}

	private MyService CreateSut()
	{
		return fixture.Create<MyService>();
	}

	[Test]
	public void When_Then()
	{
		var myDependnecy = fixture.FreezeMock<IMyDependency>();
		var sut = CreateSut();

		sut.Do();
		// ...
	}
}
```

```csharp
[TestFixture]
public class MyServiceTests : AutoFixtureTestFixture
{
    private MyService CreateSut()
    {
        return Fixture.Create<MyService>();
    }

    [Test]
    public void When_Then()
    {
        var dep = Fixture.FreezeMock<IMyDependency>();
        dep.Setup(x => x.Hello()).Returns(42);

        var sut = CreateSut();

        sut.Hello().Should().Be(42);
    }
}
```

### Embedded file

```csharp
using var stream = EmbeddedFile.GetResourceStream(Path.Combine("Resources", "someText.txt"), GetType().Assembly);
var content = await EmbeddedFile.ReadToEndAsync(Path.Combine("Resources", "someText.txt"), GetType().Assembly);

await EmbeddedFile.CopyToAsync(Path.Combine("Resources", "someText.txt"), GetType().Assembly, Path.Combine(Temp, "copied.txt"));
```