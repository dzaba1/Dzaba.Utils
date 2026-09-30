using System.Text.Json;
using System.Text.Json.Nodes;

namespace Dzaba.Utils.AspNet.Hal;

/// <summary>
/// Helpers for wrapping resources in HAL (Hypertext Application Language) envelopes.
/// </summary>
public static class HalEnvelope
{
    /// <summary>
    /// Serializes a resource and adds a <c>_links</c> section to it.
    /// </summary>
    /// <typeparam name="T">The type of the resource.</typeparam>
    /// <param name="resource">The resource to wrap. It has to serialize to a JSON object.</param>
    /// <param name="links">The links keyed by relation name.</param>
    /// <param name="options">The serializer options.</param>
    /// <returns>The resource as a JSON object with the <c>_links</c> property.</returns>
    public static JsonObject Wrap<T>(T resource, HalLinks links, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(resource);
        ArgumentNullException.ThrowIfNull(links);
        ArgumentNullException.ThrowIfNull(options);

        var node = JsonSerializer.SerializeToNode(resource, options)!.AsObject();
        node["_links"] = JsonSerializer.SerializeToNode(links, options);
        return node;
    }

    /// <summary>
    /// Builds a HAL collection with <c>_links</c> and an <c>_embedded</c> section containing the items.
    /// </summary>
    /// <typeparam name="T">The type of the items.</typeparam>
    /// <param name="embeddedKey">The name of the array under <c>_embedded</c>.</param>
    /// <param name="items">The items to embed.</param>
    /// <param name="links">The links of the collection keyed by relation name.</param>
    /// <param name="options">The serializer options.</param>
    /// <param name="itemLinks">Optional factory of per-item links. When omitted, items are embedded without <c>_links</c>.</param>
    /// <returns>The collection as a JSON object.</returns>
    public static JsonObject WrapCollection<T>(string embeddedKey, IEnumerable<T> items,
        HalLinks links, JsonSerializerOptions options,
        Func<T, HalLinks> itemLinks = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(embeddedKey);
        ArgumentNullException.ThrowIfNull(items);
        ArgumentNullException.ThrowIfNull(links);
        ArgumentNullException.ThrowIfNull(options);

        var embeddedItems = items
            .Select(item => itemLinks is null ? JsonSerializer.SerializeToNode(item, options)! : Wrap(item, itemLinks(item), options))
            .Cast<JsonNode>()
            .ToArray();

        return new JsonObject
        {
            ["_links"] = JsonSerializer.SerializeToNode(links, options),
            ["_embedded"] = new JsonObject { [embeddedKey] = new JsonArray(embeddedItems) },
        };
    }
}
