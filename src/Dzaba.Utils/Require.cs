using System;
using System.Collections.Generic;
using System.Linq;

namespace Dzaba.Utils;

/// <summary>
/// Static methods for parameters check.
/// </summary>
public static class Require
{
    /// <summary>
    /// Throws an <see cref="ArgumentNullException"/> if <paramref name="obj"/> is null.
    /// </summary>
    /// <param name="obj">The reference type argument to validate as non-null.</param>
    /// <param name="argumentName">The name of the parameter with which <paramref name="obj"/> corresponds.</param>
    /// <exception cref="ArgumentNullException"><paramref name="obj"/> is null.</exception>
    public static void NotNull(object obj, string argumentName)
    {
        if (obj == null)
        {
            throw new ArgumentNullException(argumentName);
        }
    }

    /// <summary>
    /// Throws an exception if <paramref name="str"/> is null or empty.
    /// </summary>
    /// <param name="str">The string argument to validate as non-null and non-empty.</param>
    /// <param name="argumentName">The name of the parameter with which <paramref name="str"/> corresponds.</param>
    /// <exception cref="ArgumentException"><paramref name="str"/> is empty.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="str"/> is null.</exception>
    public static void NotEmpty(string str, string argumentName)
    {
        NotNull(str, argumentName);

        if (string.IsNullOrEmpty(str))
        {
            throw new ArgumentException($"Argument {argumentName} is empty.", argumentName);
        }
    }

    /// <summary>
    /// Throws an exception if <paramref name="col"/> is null or empty.
    /// </summary>
    /// <param name="col">The enumerable argument to validate as non-null and non-empty.</param>
    /// <param name="argumentName">The name of the parameter with which <paramref name="col"/> corresponds.</param>
    /// <exception cref="ArgumentNullException"><paramref name="col"/> is null.</exception>
    /// <exception cref="ArgumentException"><paramref name="col"/> is empty.</exception>
    public static void NotEmpty<T>(IEnumerable<T> col, string argumentName)
    {
        NotNull(col, argumentName);

        if (!col.Any())
        {
            throw new ArgumentException($"Argument {argumentName} is empty.", argumentName);
        }
    }

    /// <summary>
    /// Throws an exception if <paramref name="str"/> is null, empty or white space.
    /// </summary>
    /// <param name="str">The string argument to validate as non-null, non-empty and non-white-space.</param>
    /// <param name="argumentName">The name of the parameter with which <paramref name="str"/> corresponds.</param>
    /// <exception cref="ArgumentException"><paramref name="str"/> is empty or white space.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="str"/> is null.</exception>
    public static void NotWhiteSpace(string str, string argumentName)
    {
        NotNull(str, argumentName);

        if (string.IsNullOrWhiteSpace(str))
        {
            throw new ArgumentException($"Argument {argumentName} is empty or white space.", argumentName);
        }
    }
}
