using System.Text.Json.Serialization;

namespace Dzaba.Utils.AspNet.Hal;

/// <summary>
/// Represents a HAL link to a related resource or action.
/// </summary>
/// <param name="Href">The target URL of the link.</param>
/// <param name="Method">The optional HTTP method to use when following the link.</param>
public sealed record HalLink(
    [property: JsonPropertyName("href")] string Href,
    [property: JsonPropertyName("method")] string Method = null);
