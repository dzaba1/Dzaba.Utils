using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;

namespace Dzaba.Utils.AspNet.Hal;

/// <summary>
/// A set of HAL links keyed by relation name. It has typed properties for <c>self</c> and for every standard HTTP method
/// (<c>get</c>, <c>post</c>, <c>put</c>, <c>patch</c>, <c>delete</c>, <c>head</c>, <c>options</c>, <c>trace</c>, <c>connect</c>),
/// and an indexer or <see cref="Add"/> for any custom relation. Each relation is a list of links: when it has several,
/// they are serialized as an array of link objects (use <see cref="HalLink.Name"/> to tell them apart); when it has a single one,
/// it is serialized as a single link object. Relations without links are left out of the output.
/// </summary>
[JsonConverter(typeof(HalLinksConverter))]
public sealed class HalLinks : IReadOnlyDictionary<string, IReadOnlyList<HalLink>>
{
    private readonly Dictionary<string, List<HalLink>> _links = [];

    /// <summary>
    /// Gets or sets the links of the <c>self</c> relation. See <see cref="this[string]"/> for the semantics.
    /// </summary>
    public List<HalLink> Self { get => this["self"]; set => this["self"] = value; }

    /// <summary>
    /// Gets or sets the links of the <c>get</c> relation. See <see cref="this[string]"/> for the semantics.
    /// </summary>
    public List<HalLink> Get { get => this["get"]; set => this["get"] = value; }

    /// <summary>
    /// Gets or sets the links of the <c>post</c> relation. See <see cref="this[string]"/> for the semantics.
    /// </summary>
    public List<HalLink> Post { get => this["post"]; set => this["post"] = value; }

    /// <summary>
    /// Gets or sets the links of the <c>put</c> relation. See <see cref="this[string]"/> for the semantics.
    /// </summary>
    public List<HalLink> Put { get => this["put"]; set => this["put"] = value; }

    /// <summary>
    /// Gets or sets the links of the <c>patch</c> relation. See <see cref="this[string]"/> for the semantics.
    /// </summary>
    public List<HalLink> Patch { get => this["patch"]; set => this["patch"] = value; }

    /// <summary>
    /// Gets or sets the links of the <c>delete</c> relation. See <see cref="this[string]"/> for the semantics.
    /// </summary>
    public List<HalLink> Delete { get => this["delete"]; set => this["delete"] = value; }

    /// <summary>
    /// Gets or sets the links of the <c>head</c> relation. See <see cref="this[string]"/> for the semantics.
    /// </summary>
    public List<HalLink> Head { get => this["head"]; set => this["head"] = value; }

    /// <summary>
    /// Gets or sets the links of the <c>options</c> relation. See <see cref="this[string]"/> for the semantics.
    /// </summary>
    public List<HalLink> Options { get => this["options"]; set => this["options"] = value; }

    /// <summary>
    /// Gets or sets the links of the <c>trace</c> relation. See <see cref="this[string]"/> for the semantics.
    /// </summary>
    public List<HalLink> Trace { get => this["trace"]; set => this["trace"] = value; }

    /// <summary>
    /// Gets or sets the links of the <c>connect</c> relation. See <see cref="this[string]"/> for the semantics.
    /// </summary>
    public List<HalLink> Connect { get => this["connect"]; set => this["connect"] = value; }

    /// <summary>
    /// Gets or sets the links of a relation, including custom ones. Getting never returns <see langword="null"/>:
    /// a relation without links gives an empty list that can be filled in place (<c>links.Delete.Add(link)</c>,
    /// or <c>Delete = { a, b }</c> in an initializer). Setting stores the given list, and setting <see langword="null"/>
    /// removes the relation. A relation with an empty list is left out of the output.
    /// </summary>
    /// <param name="rel">The relation name.</param>
    public List<HalLink> this[string rel]
    {
        get
        {
            ArgumentException.ThrowIfNullOrEmpty(rel);

            if (!_links.TryGetValue(rel, out var links))
            {
                links = [];
                _links[rel] = links;
            }

            return links;
        }
        set
        {
            ArgumentException.ThrowIfNullOrEmpty(rel);

            if (value is null)
            {
                _links.Remove(rel);
            }
            else
            {
                _links[rel] = value;
            }
        }
    }

    /// <summary>
    /// Adds a link to a relation, keeping the links already there. It also makes the class usable with a collection initializer.
    /// </summary>
    /// <param name="rel">The relation name.</param>
    /// <param name="link">The link.</param>
    public void Add(string rel, HalLink link)
    {
        ArgumentNullException.ThrowIfNull(link);

        this[rel].Add(link);
    }

    private IEnumerable<KeyValuePair<string, List<HalLink>>> NonEmpty => _links.Where(p => p.Value.Count > 0);

    /// <inheritdoc />
    public IEnumerable<string> Keys => NonEmpty.Select(p => p.Key);

    /// <inheritdoc />
    public IEnumerable<IReadOnlyList<HalLink>> Values => NonEmpty.Select(p => (IReadOnlyList<HalLink>)p.Value);

    /// <inheritdoc />
    public int Count => NonEmpty.Count();

    /// <inheritdoc />
    public bool ContainsKey(string key) => _links.TryGetValue(key, out var links) && links.Count > 0;

    /// <inheritdoc />
    public bool TryGetValue(string key, [MaybeNullWhen(false)] out IReadOnlyList<HalLink> value)
    {
        if (_links.TryGetValue(key, out var links) && links.Count > 0)
        {
            value = links;
            return true;
        }

        value = null;
        return false;
    }

    /// <inheritdoc />
    public IEnumerator<KeyValuePair<string, IReadOnlyList<HalLink>>> GetEnumerator() =>
        NonEmpty.Select(p => new KeyValuePair<string, IReadOnlyList<HalLink>>(p.Key, p.Value)).GetEnumerator();

    IReadOnlyList<HalLink> IReadOnlyDictionary<string, IReadOnlyList<HalLink>>.this[string key] =>
        TryGetValue(key, out var links) ? links : throw new KeyNotFoundException($"The relation '{key}' has no links.");

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
