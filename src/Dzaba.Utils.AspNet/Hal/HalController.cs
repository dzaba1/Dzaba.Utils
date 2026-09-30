using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace Dzaba.Utils.AspNet.Hal;

/// <summary>
/// Base class for controllers that return HAL responses. It exposes an <see cref="ILinkFactory"/> for building links
/// and the JSON serializer options used by the MVC pipeline, so both can be passed to <see cref="HalEnvelope"/>.
/// </summary>
public abstract class HalController : ControllerBase
{
    /// <summary>
    /// Gets the factory used to build HAL links to controller actions.
    /// </summary>
    protected ILinkFactory Links { get; }

    /// <summary>
    /// Gets the JSON serializer options to use when wrapping resources with <see cref="HalEnvelope"/>.
    /// </summary>
    protected JsonSerializerOptions JsonOptions { get; }

    /// <summary>
    /// Initializes a new instance of the HalController class.
    /// </summary>
    /// <param name="links">The factory used to build HAL links.</param>
    /// <param name="jsonOptions">The MVC JSON options. Their <see cref="Microsoft.AspNetCore.Mvc.JsonOptions.JsonSerializerOptions"/> are
    /// exposed through the <see cref="JsonOptions"/> property.</param>
    protected HalController(ILinkFactory links, JsonOptions jsonOptions)
    {
        ArgumentNullException.ThrowIfNull(links);
        ArgumentNullException.ThrowIfNull(jsonOptions);

        Links = links;
        JsonOptions = jsonOptions.JsonSerializerOptions;
    }

    /// <summary>
    /// Creates a builder of HAL links for the current request. It uses the <see cref="ControllerBase.HttpContext"/>
    /// of the controller and the <see cref="Links"/> factory.
    /// </summary>
    /// <returns>A new <see cref="HalLinksBuilder"/>.</returns>
    protected HalLinksBuilder CreateLinksBuilder() => new(HttpContext, Links);

    /// <summary>
    /// Creates an <see cref="OkObjectResult"/> with the resource wrapped in a HAL envelope by <see cref="HalEnvelope.Wrap{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the resource.</typeparam>
    /// <param name="resource">The resource to wrap. It has to serialize to a JSON object.</param>
    /// <param name="links">The links of the resource.</param>
    /// <returns>The 200 OK result.</returns>
    protected OkObjectResult OkHal<T>(T resource, HalLinks links) =>
        Ok(HalEnvelope.Wrap(resource, links, JsonOptions));

    /// <summary>
    /// Creates an <see cref="OkObjectResult"/> with a HAL collection built by <see cref="HalEnvelope.WrapCollection{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the items.</typeparam>
    /// <param name="embeddedKey">The name of the array under <c>_embedded</c>.</param>
    /// <param name="items">The items to embed.</param>
    /// <param name="links">The links of the collection.</param>
    /// <param name="itemLinks">Optional factory of per-item links. When omitted, items are embedded without <c>_links</c>.</param>
    /// <returns>The 200 OK result.</returns>
    protected OkObjectResult OkHal<T>(string embeddedKey, IEnumerable<T> items, HalLinks links, Func<T, HalLinks> itemLinks = null) =>
        Ok(HalEnvelope.WrapCollection(embeddedKey, items, links, JsonOptions, itemLinks));
}
