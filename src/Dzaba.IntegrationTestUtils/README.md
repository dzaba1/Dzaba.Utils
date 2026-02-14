# Dzaba.IntegrationTestUtils

Utilities for integration tests.

## Usage

```csharp
[TestFixture]
public class MyServiceTest : IocTestFixture
{
	protected override void RegisterServices(ServiceCollection services)
    {
        services.AddTransient<IMyService, MyService>();
    }

    [Test]
    public void DoWorkTest()
    {
        var service = Container.GetService<IMyService>();
        service.DoWork();
    }
}
```
