using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Collections.Generic;

namespace Dzaba.Utils.Configuration.Tests;

[TestFixture]
public class ConfigurationExtensionsTests
{
    private IConfiguration CreateConfiguration()
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection([
                new KeyValuePair<string, string>("myString", "myString"),
                new KeyValuePair<string, string>("Data:Name", "name"),
                new KeyValuePair<string, string>("Data:Id", "1"),
                ])
            .Build();
    }

    [Test]
    public void RegisterDzabaConfiguration_WhenConfigurationObject_ThenSingleton()
    {
        var services = new ServiceCollection();
        var config = CreateConfiguration();
        services.RegisterDzabaConfiguration(config);

        using var provider = services.BuildServiceProvider();
        var configuration1 = provider.GetRequiredService<IConfiguration>();
        var configuration2 = provider.GetRequiredService<IConfiguration>();
        configuration1.Should().BeSameAs(configuration2);
    }

    [Test]
    public void RegisterDzabaConfiguration_WhenConfigurationBuilderAsSingleton_ThenSingleton()
    {
        var services = new ServiceCollection();
        services.RegisterDzabaConfiguration(p => CreateConfiguration(), true);

        using var provider = services.BuildServiceProvider();
        var configuration1 = provider.GetRequiredService<IConfiguration>();
        var configuration2 = provider.GetRequiredService<IConfiguration>();
        configuration1.Should().BeSameAs(configuration2);
    }

    [Test]
    public void RegisterDzabaConfiguration_WhenConfigurationBuilderAsTransient_ThenNewServices()
    {
        var services = new ServiceCollection();
        services.RegisterDzabaConfiguration(p => CreateConfiguration());

        using var provider = services.BuildServiceProvider();
        var configuration1 = provider.GetRequiredService<IConfiguration>();
        var configuration2 = provider.GetRequiredService<IConfiguration>();
        configuration1.Should().NotBeSameAs(configuration2);
    }

    [Test]
    public void RegisterDzabaSettings_WhenSectionExists_ThenModel()
    {
        var services = new ServiceCollection();
        var config = CreateConfiguration();
        services.RegisterDzabaConfiguration(config);
        services.RegisterDzabaSettings<TestSettings>("Data");

        using var provider = services.BuildServiceProvider();
        var settings = provider.GetRequiredService<TestSettings>();
        settings.Name.Should().Be("name");
        settings.Id.Should().Be(1);
    }

    private class TestSettings
    {
        public string Name { get; set; }
        public int Id { get; set; }
    }
}
