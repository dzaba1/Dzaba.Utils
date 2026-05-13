using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Dzaba.TestUtils.Integration.AspNet;

/// <summary>
/// Base class for testing ASP.NET controllers. Provides a <see cref="WebApplicationFactory{T}"/> and helper methods for making requests and asserting responses.
/// </summary>
public abstract class ControllerTestFixture<T> : TempTestFixture
    where T : class
{
    /// <summary>
    /// The mocked JWT settings used in tests
    /// </summary>
    protected JwtMockedSettings JwtSettings { get; } = new JwtMockedSettings();

    /// <summary>
    /// The <see cref="WebApplicationFactory{T}"/> used to create test clients and scopes. Configured in <see cref="SetupFactory"/> and disposed in <see cref="CleanupFactory"/>.
    /// </summary>
    protected WebApplicationFactory<T> Factory { get; private set; }

    /// <summary>
    /// Override to configure the application configuration for the test server. Called during <see cref="SetupFactory"/> when configuring the web host builder.
    /// </summary>
    /// <param name="builder">Configuration builder</param>
    protected virtual void OnConfigureConfiguration(IConfigurationBuilder builder)
    {
        
    }

    /// <summary>
    /// Override to configure the services for the test server. Called during <see cref="SetupFactory"/> when configuring the web host builder.
    /// </summary>
    /// <param name="services">Service collection</param>
    protected virtual void OnConfigureServices(IServiceCollection services)
    {
        
    }

    /// <summary>
    /// Sets up the <see cref="Factory"/> by creating a new <see cref="WebApplicationFactory{T}"/> and configuring it with the provided configuration and services. This method is called before each test to ensure a fresh factory instance.
    /// </summary>
    [SetUp]
    public void SetupFactory()
    {
        Factory = new WebApplicationFactory<T>().WithWebHostBuilder(b =>
        {
            b.ConfigureAppConfiguration((context, builder) =>
            {
                OnConfigureConfiguration(builder);
            })
            .ConfigureTestServices(services =>
            {
                services.AddSerilogConsoleLogging();

                OnConfigureServices(services);
            });
        });
    }

    /// <summary>
    /// Cleans up the <see cref="Factory"/> by disposing it. This method is called after each test to ensure that resources are released and there are no side effects between tests.
    /// </summary>
    [TearDown]
    public void CleanupFactory()
    {
        Factory?.Dispose();
    }

    /// <summary>
    /// Gets the service provider from the test factory, allowing access to registered services for assertions or setup. This is a convenient property for accessing services without needing to create a scope manually.
    /// </summary>
    protected IServiceProvider Container => Factory.Services;

    /// <summary>
    /// Creates a new service scope for the test factory.
    /// </summary>
    /// <returns>The created service scope.</returns>
    protected IServiceScope CreateScope()
    {
        return Factory.Services.CreateScope();
    }

    /// <summary>
    /// Creates a new HTTP client for the test factory.
    /// </summary>
    /// <returns>The created HTTP client.</returns>
    protected HttpClient CreateClient()
    {
        return Factory.CreateClient();
    }

    /// <summary>
    /// Asserts that the given HTTP response is successful (status code 2xx) and returns the response content as a string. If the response is not successful, an assertion failure will be thrown with the response content included in the message for easier debugging.
    /// </summary>
    /// <param name="resp">The HTTP response to assert.</param>
    /// <returns>The response content as a string.</returns>
    protected async Task<string> AssertAsync(HttpResponseMessage resp)
    {
        resp.Should().NotBeNull();
        var respStr = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);

        this.Invoking(_ => resp.EnsureSuccessStatusCode())
            .Should().NotThrow(respStr);

        return respStr;
    }
}