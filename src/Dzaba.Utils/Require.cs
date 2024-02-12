using System;
using System.Collections.Generic;
using System.Linq;

namespace Dzaba.Utils;

public static class Require
{
    public static void NotNull(object obj, string argumentName)
    {
        if (obj == null)
        {
            throw new ArgumentNullException(argumentName);
        }
    }

    public static void NotEmpty(string str, string argumentName)
    {
        if (string.IsNullOrEmpty(str))
        {
            throw new ArgumentNullException(argumentName);
        }
    }

    public static void NotEmpty<T>(IEnumerable<T> col, string argumentName)
    {
        if (col == null || !col.Any())
        {
            throw new ArgumentNullException(argumentName);
        }
    }

    public static void NotWhiteSpace(string str, string argumentName)
    {
        if (string.IsNullOrWhiteSpace(str))
        {
            throw new ArgumentNullException(argumentName);
        }
    }
}
