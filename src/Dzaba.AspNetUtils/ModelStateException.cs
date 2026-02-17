namespace Dzaba.AspNetUtils;

/// <summary>
/// Represents an exception that is thrown when model validation fails and one or more validation errors are
/// encountered.
/// </summary>
public class ModelStateException : Exception
{
    /// <summary>
    /// Gets an array of key-value pairs that represent the recorded error messages, where each key identifies the error
    /// and each value contains the corresponding error message.
    /// </summary>
    public KeyValuePair<string, string>[] Errors { get; }

    /// <summary>
    /// Initializes a new instance of the ModelStateException class with a specified error message, a collection of
    /// validation errors, and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="errors">A collection of key-value pairs representing validation errors, where each key is the name of the property that
    /// failed validation and the value is the associated error message. Cannot be null.</param>
    public ModelStateException(IEnumerable<KeyValuePair<string, string>> errors)
    {
        ArgumentNullException.ThrowIfNull(errors);

        Errors = errors.ToArray();
    }

    /// <summary>
    /// Initializes a new instance of the ModelStateException class with a specified error message, a collection of
    /// validation errors, and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="errors">A collection of key-value pairs representing validation errors, where each key is the name of the property that
    /// failed validation and the value is the associated error message. Cannot be null.</param>
    public ModelStateException(string message, IEnumerable<KeyValuePair<string, string>> errors)
        : base(message)
    {
        ArgumentNullException.ThrowIfNull(errors);

        Errors = errors.ToArray();
    }

    /// <summary>
    /// Initializes a new instance of the ModelStateException class with a specified error message, a collection of
    /// validation errors, and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="errors">A collection of key-value pairs representing validation errors, where each key is the name of the property that
    /// failed validation and the value is the associated error message. Cannot be null.</param>
    /// <param name="inner">The exception that is the cause of the current exception, or null if no inner exception is specified.</param>
    public ModelStateException(string message, IEnumerable<KeyValuePair<string, string>> errors, Exception inner)
        : base(message, inner)
    {
        ArgumentNullException.ThrowIfNull(errors);

        Errors = errors.ToArray();
    }
}
