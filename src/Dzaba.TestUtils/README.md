# Dzaba.TestUtils

Utilities for unit and integration tests.

## Usage

```
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