using System.Text.Json.Serialization;

namespace Dzaba.Utils.AspNet.Hal;

/// <summary>
/// Represents a HAL link to a related resource or action.
/// </summary>
/// <param name="Href">The target URL of the link.</param>
/// <param name="Method">The optional HTTP method to use when following the link.</param>
/// <param name="Name">The optional name used as a secondary key to tell apart several links of the same relation.
/// It is left out of the output when null.</param>
public sealed record HalLink(
    [property: JsonPropertyName("href")] string Href,
    [property: JsonPropertyName("method")] string Method = null,
    [property: JsonPropertyName("name"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] string Name = null);
