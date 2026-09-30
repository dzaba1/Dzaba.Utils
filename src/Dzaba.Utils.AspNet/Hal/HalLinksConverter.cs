using System.Text.Json;
using System.Text.Json.Serialization;

namespace Dzaba.Utils.AspNet.Hal;

/// <summary>
/// Writes <see cref="HalLinks"/> as a HAL <c>_links</c> object: a relation with one link becomes a link object,
/// a relation with several links becomes an array of link objects.
/// </summary>
internal sealed class HalLinksConverter : JsonConverter<HalLinks>
{
    public override HalLinks Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        throw new NotSupportedException($"Reading {nameof(HalLinks)} is not supported.");

    public override void Write(Utf8JsonWriter writer, HalLinks value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        foreach (var (rel, links) in value)
        {
            writer.WritePropertyName(rel);

            if (links.Count == 1)
            {
                JsonSerializer.Serialize(writer, links[0], options);
                continue;
            }

            writer.WriteStartArray();
            foreach (var link in links)
            {
                JsonSerializer.Serialize(writer, link, options);
            }
            writer.WriteEndArray();
        }

        writer.WriteEndObject();
    }
}
