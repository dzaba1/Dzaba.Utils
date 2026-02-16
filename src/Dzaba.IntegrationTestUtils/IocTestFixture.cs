using Dzaba.TestUtils;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;

namespace Dzaba.IntegrationTestUtils;

/// <summary>
/// Provides an abstract base class for integration tests that require a dependency injection container with logging
/// support.
/// </summary>
public abstract class IocTestFixture : TempTestFixture
{
    private ServiceProvider container;

    /// <summary>
    /// Gets the service provider that supplies access to application services and dependencies.
    /// </summary>
    protected IServiceProvider Container => container;

    /// <summary>
    /// Initializes the dependency injection container with logging and service registrations.
    /// </summary>
    [SetUp]
    public void SetupContainer()
    {
        var services = new ServiceCollection();
        services.AddSerilogConsoleLogging();

        RegisterServices(services);

        container = services.BuildServiceProvider();
    }

    /// <summary>
    /// Registers application services with the specified service collection.
    /// </summary>
    protected abstract void RegisterServices(ServiceCollection services);

    /// <summary>
    /// Releases resources held by the container after each test execution.
    /// </summary>
    [TearDown]
    public void CleanupContainer()
    {
        container?.Dispose();
    }
}
