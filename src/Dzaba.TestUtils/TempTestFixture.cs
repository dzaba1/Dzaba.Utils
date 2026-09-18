using NUnit.Framework;
using System;
using System.IO;
using System.Threading;

namespace Dzaba.TestUtils;

/// <summary>
/// Provides a base class for test fixtures that manage a temporary directory for test execution.
/// </summary>
public abstract class TempTestFixture
{
    /// <summary>
    /// Gets the temporary string value used for internal processing.
    /// </summary>
    protected string Temp { get; private set; }

    /// <summary>
    /// Initializes a temporary directory for use by all tests in the class. This method is called once before any tests
    /// are executed. A random suffix is appended to the type name so that fixtures with the same name running in
    /// parallel, e.g. the same tests executed for multiple target frameworks, do not share the same temp directory.
    /// </summary>
    [OneTimeSetUp]
    public void OneTimeTempSetup()
    {
        Temp = Path.Combine(Path.GetTempPath(), $"{GetType().Name}_{GetTempSuffix()}");
    }

    /// <summary>
    /// Gets the suffix appended to the temp directory name. Overridable so a deterministic value can be injected,
    /// e.g. in tests that need a stable path. Defaults to a new random GUID.
    /// </summary>
    protected virtual string GetTempSuffix()
    {
        return Guid.NewGuid().ToString("N");
    }

    /// <summary>
    /// Removes the temporary directory and its contents if it exists after test execution. Retries briefly on
    /// <see cref="IOException"/> because file handles, e.g. from disposed loggers or hosted servers, can be
    /// released asynchronously by the OS after the owning object's Dispose call returns.
    /// </summary>
    [TearDown]
    public void CleanupTemp()
    {
        const int maxAttempts = 5;

        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                if (Directory.Exists(Temp))
                {
                    Directory.Delete(Temp, true);
                }

                return;
            }
            catch (IOException) when (attempt < maxAttempts)
            {
                Thread.Sleep(50 * attempt);
            }
            catch (UnauthorizedAccessException) when (attempt < maxAttempts)
            {
                Thread.Sleep(50 * attempt);
            }
        }
    }

    /// <summary>
    /// Initializes the temporary directory for testing by removing any existing temporary files and creating a new
    /// directory to ensure a clean environment for each test run.
    /// </summary>
    [SetUp]
    public void SetupTemp()
    {
        CleanupTemp();
        Directory.CreateDirectory(Temp);
    }
}
