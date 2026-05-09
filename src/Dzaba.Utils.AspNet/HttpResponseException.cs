using System.Net;
using System.Text;

namespace Dzaba.AspNetUtils;

/// <summary>
/// Represents an exception that is thrown to indicate an error response from an HTTP request, including the associated
/// HTTP status code.
/// </summary>
public class HttpResponseException : Exception
{
    /// <summary>
    /// Gets the HTTP status code returned by the server in response to the HTTP request.
    /// </summary>
    public HttpStatusCode StatusCode { get; }

    /// <summary>
    /// Initializes a new instance of the HttpResponseException class with the specified HTTP status code.
    /// </summary>
    /// <param name="statusCode">The HTTP status code that represents the error condition to be returned to the client.</param>
    public HttpResponseException(HttpStatusCode statusCode)
    {
        StatusCode = statusCode;
    }

    /// <summary>
    /// Initializes a new instance of the HttpResponseException class with the specified HTTP status code.
    /// </summary>
    /// <param name="statusCode">The HTTP status code that represents the error to be returned to the client.</param>
    /// <param name="message">The error message that describes the reason for the exception.</param>
    public HttpResponseException(HttpStatusCode statusCode, string message)
        : base(message)
    {
        StatusCode = statusCode;
    }

    /// <summary>
    /// Initializes a new instance of the HttpResponseException class with the specified HTTP status code.
    /// </summary>
    /// <param name="statusCode">The HTTP status code that represents the error to be returned to the client.</param>
    /// <param name="message">The error message that describes the reason for the exception.</param>
    /// <param name="inner">The exception that is the cause of the current exception, or null if no inner exception is specified.</param>
    public HttpResponseException(HttpStatusCode statusCode, string message, Exception inner)
        : base(message, inner)
    {
        StatusCode = statusCode;
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        var builder = new StringBuilder();
        if (!string.IsNullOrWhiteSpace(Message))
        {
            builder.AppendLine(Message);
        }

        builder.AppendLine($"HTTP code: {StatusCode}");

        if (!string.IsNullOrWhiteSpace(StackTrace))
        {
            builder.AppendLine(StackTrace);
        }

        if (InnerException != null)
        {
            builder.AppendLine();
            builder.Append("Inner exception: ");
            builder.Append(InnerException);
        }

        return builder.ToString();
    }
}
