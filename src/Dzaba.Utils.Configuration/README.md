# Dzaba.TestUtils

Utilities for Microsoft.Extensions.Configuration and DI

## Usage

```
services.AddConfiguration(Configuration);

services.AddConfiguration(c =>
{
    var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
    return new ConfigurationBuilder()
        .AddJsonFile(path)
        .Build();
});

services.AddJsonFileConfiguration();

services.AddInMemoryConfiguration(new KeyValuePair<string,string>("MySection:MyValue", "MyValue));

services.AddSettings<MySettings>("mySettingsSection");
```
