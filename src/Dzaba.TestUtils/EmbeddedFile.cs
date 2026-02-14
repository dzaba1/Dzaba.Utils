using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Dzaba.TestUtils;

/// <summary>
/// Provides methods for retrieving embedded resource streams from .NET assemblies.
/// </summary>
public static class EmbeddedFile
{
    /// <summary>
    /// Retrieves an embedded resource stream from the specified assembly using the provided resource file name.
    /// </summary>
    /// <remarks>The resource name is constructed by prefixing the file name with the assembly's name and
    /// replacing any path separators with dots, following the convention for embedded resources in .NET
    /// assemblies.</remarks>
    /// <param name="file">The name of the resource file to retrieve. This value must not be null, empty, or consist only of white-space
    /// characters.</param>
    /// <param name="assembly">The assembly from which to retrieve the embedded resource. This value must not be null.</param>
    /// <returns>A stream containing the resource data. The stream is guaranteed to be non-null if the resource is found.</returns>
    /// <exception cref="FileNotFoundException">Thrown if a resource with the specified name cannot be found in the provided assembly.</exception>
    public static Stream GetResourceStream(string file, Assembly assembly)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(file);
        ArgumentNullException.ThrowIfNull(assembly);

        var assemblyName = assembly.GetName().Name;
        var formattedFile = file.Replace(Path.DirectorySeparatorChar, '.').Replace(Path.AltDirectorySeparatorChar, '.');
        var fullFile = $"{assemblyName}.{formattedFile}";

        var stream = assembly.GetManifestResourceStream(fullFile);
        if (stream is null)
        {
            throw new FileNotFoundException($"Resource with name '{fullFile}' not found.", fullFile);
        }

        return stream;
    }

    /// <summary>
    /// Asynchronously reads the entire content of the specified embedded resource file from the given assembly.
    /// </summary>
    /// <param name="file">The name of the embedded resource file to read. This parameter cannot be null or empty.</param>
    /// <param name="assembly">The assembly that contains the embedded resource file. This parameter must not be null.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the content of the resource file as
    /// a string.</returns>
    public static async Task<string> ReadToEndAsync(string file, Assembly assembly)
    {
        using var stream = GetResourceStream(file, assembly);
        using var reader = new StreamReader(stream);
        return await reader.ReadToEndAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously copies the contents of an embedded resource file from the specified assembly to the provided
    /// target stream.
    /// </summary>
    /// <param name="file">The name of the embedded resource file to copy from the assembly.</param>
    /// <param name="assembly">The assembly that contains the embedded resource file.</param>
    /// <param name="target">The stream to which the contents of the resource file will be copied. This parameter cannot be null.</param>
    /// <returns>A task that represents the asynchronous copy operation.</returns>
    public static async Task CopyToAsync(string file, Assembly assembly, Stream target)
    {
        ArgumentNullException.ThrowIfNull(target);

        using var stream = GetResourceStream(file, assembly);
        await stream.CopyToAsync(target).ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronously copies the contents of an embedded resource file from the specified assembly to a target file on
    /// disk.
    /// </summary>
    /// <param name="file">The name of the resource file to copy from the provided assembly. This should be the full resource name as
    /// embedded in the assembly.</param>
    /// <param name="assembly">The assembly that contains the embedded resource file to be copied.</param>
    /// <param name="targetFile">The path to the file where the resource content will be written. Cannot be null, empty, or consist only of
    /// white-space characters.</param>
    /// <returns>A task that represents the asynchronous copy operation.</returns>
    public static async Task CopyToAsync(string file, Assembly assembly, string targetFile)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(targetFile);

        using var stream = GetResourceStream(file, assembly);
        using var target = File.Create(targetFile);

        await stream.CopyToAsync(target).ConfigureAwait(false);
    }
}
