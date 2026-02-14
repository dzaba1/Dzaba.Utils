using NUnit.Framework;
using System.IO;

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
    /// are executed.
    /// </summary>
    [OneTimeSetUp]
    public void OneTimeTempSetup()
    {
        Temp = Path.Combine(Path.GetTempPath(), GetType().Name);
    }

    /// <summary>
    /// Removes the temporary directory and its contents if it exists after test execution.
    /// </summary>
    [TearDown]
    public void CleanupTemp()
    {
        if (Directory.Exists(Temp))
        {
            Directory.Delete(Temp, true);
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
