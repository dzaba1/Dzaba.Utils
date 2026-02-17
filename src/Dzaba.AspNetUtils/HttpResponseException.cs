using System.Net;
using System.Text;

namespace Dzaba.AspNetUtils;

public class HttpResponseException : Exception
{
    public HttpStatusCode StatusCode { get; }

    public HttpResponseException(HttpStatusCode statusCode)
    {
        StatusCode = statusCode;
    }

    public HttpResponseException(HttpStatusCode statusCode, string message)
        : base(message)
    {
        StatusCode = statusCode;
    }

    public HttpResponseException(HttpStatusCode statusCode, string message, Exception inner)
        : base(message, inner)
    {
        StatusCode = statusCode;
    }

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
