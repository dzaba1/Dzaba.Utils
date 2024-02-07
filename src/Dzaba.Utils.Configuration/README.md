# Dzaba.TestUtils

Utilities for Microsoft.Extensions.Configuration and DI

## Usage

```
services.RegisterDzabaConfiguration(Configuration);

services.RegisterDzabaConfiguration(c =>
{
    var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
    return new ConfigurationBuilder()
        .AddJsonFile(path)
        .Build();
});

services.RegisterDzabaSettings<MySettings>("mySettingsSection");
```
