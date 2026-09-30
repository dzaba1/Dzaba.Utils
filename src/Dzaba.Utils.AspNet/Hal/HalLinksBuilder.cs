using Microsoft.AspNetCore.Http;

namespace Dzaba.Utils.AspNet.Hal;

/// <summary>
/// Builds <see cref="HalLinks"/> for the current HTTP request. Each Add method builds a link to a controller action
/// with <see cref="ILinkFactory"/> and adds it under the matching relation. Calling one for the same relation again
/// adds another link to it (use <c>name</c> to tell them apart).
/// </summary>
public sealed class HalLinksBuilder
{
    private readonly HttpContext httpContext;
    private readonly ILinkFactory linkFactory;
    private readonly HalLinks links = new();

    /// <summary>
    /// Initializes a new instance of the HalLinksBuilder class.
    /// </summary>
    /// <param name="httpContext">The current HTTP context.</param>
    /// <param name="linkFactory">The factory used to build the links.</param>
    public HalLinksBuilder(HttpContext httpContext, ILinkFactory linkFactory)
    {
        ArgumentNullException.ThrowIfNull(httpContext);
        ArgumentNullException.ThrowIfNull(linkFactory);

        this.httpContext = httpContext;
        this.linkFactory = linkFactory;
    }

    /// <summary>
    /// Adds a link to a controller action under a custom relation.
    /// </summary>
    /// <param name="rel">The relation name.</param>
    /// <param name="action">The name of the action.</param>
    /// <param name="controller">The name of the controller.</param>
    /// <param name="routeValues">Optional route values used to generate the URL.</param>
    /// <param name="method">The optional HTTP method to advertise in the link.</param>
    /// <param name="name">The optional name of the link, to tell it apart from other links of the same relation.</param>
    /// <returns>This builder.</returns>
    public HalLinksBuilder Add(string rel, string action, string controller,
        object routeValues = null, HttpMethod method = null, string name = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(rel);

        var link = linkFactory.Build(httpContext, action, controller, routeValues, method);
        if (name is not null)
        {
            link = link with { Name = name };
        }

        links.Add(rel, link);
        return this;
    }

    /// <summary>
    /// Adds a <c>self</c> link, without an HTTP method.
    /// </summary>
    /// <inheritdoc cref="Add" path="/param[@name='action' or @name='controller' or @name='routeValues' or @name='name']"/>
    /// <returns>This builder.</returns>
    public HalLinksBuilder AddSelf(string action, string controller, object routeValues = null, string name = null) =>
        Add("self", action, controller, routeValues, null, name);

    /// <summary>
    /// Adds a <c>get</c> link with the GET method.
    /// </summary>
    /// <inheritdoc cref="Add" path="/param[@name='action' or @name='controller' or @name='routeValues' or @name='name']"/>
    /// <returns>This builder.</returns>
    public HalLinksBuilder AddGet(string action, string controller, object routeValues = null, string name = null) =>
        Add("get", action, controller, routeValues, HttpMethod.Get, name);

    /// <summary>
    /// Adds a <c>post</c> link with the POST method.
    /// </summary>
    /// <inheritdoc cref="Add" path="/param[@name='action' or @name='controller' or @name='routeValues' or @name='name']"/>
    /// <returns>This builder.</returns>
    public HalLinksBuilder AddPost(string action, string controller, object routeValues = null, string name = null) =>
        Add("post", action, controller, routeValues, HttpMethod.Post, name);

    /// <summary>
    /// Adds a <c>put</c> link with the PUT method.
    /// </summary>
    /// <inheritdoc cref="Add" path="/param[@name='action' or @name='controller' or @name='routeValues' or @name='name']"/>
    /// <returns>This builder.</returns>
    public HalLinksBuilder AddPut(string action, string controller, object routeValues = null, string name = null) =>
        Add("put", action, controller, routeValues, HttpMethod.Put, name);

    /// <summary>
    /// Adds a <c>patch</c> link with the PATCH method.
    /// </summary>
    /// <inheritdoc cref="Add" path="/param[@name='action' or @name='controller' or @name='routeValues' or @name='name']"/>
    /// <returns>This builder.</returns>
    public HalLinksBuilder AddPatch(string action, string controller, object routeValues = null, string name = null) =>
        Add("patch", action, controller, routeValues, HttpMethod.Patch, name);

    /// <summary>
    /// Adds a <c>delete</c> link with the DELETE method.
    /// </summary>
    /// <inheritdoc cref="Add" path="/param[@name='action' or @name='controller' or @name='routeValues' or @name='name']"/>
    /// <returns>This builder.</returns>
    public HalLinksBuilder AddDelete(string action, string controller, object routeValues = null, string name = null) =>
        Add("delete", action, controller, routeValues, HttpMethod.Delete, name);

    /// <summary>
    /// Adds a <c>head</c> link with the HEAD method.
    /// </summary>
    /// <inheritdoc cref="Add" path="/param[@name='action' or @name='controller' or @name='routeValues' or @name='name']"/>
    /// <returns>This builder.</returns>
    public HalLinksBuilder AddHead(string action, string controller, object routeValues = null, string name = null) =>
        Add("head", action, controller, routeValues, HttpMethod.Head, name);

    /// <summary>
    /// Adds an <c>options</c> link with the OPTIONS method.
    /// </summary>
    /// <inheritdoc cref="Add" path="/param[@name='action' or @name='controller' or @name='routeValues' or @name='name']"/>
    /// <returns>This builder.</returns>
    public HalLinksBuilder AddOptions(string action, string controller, object routeValues = null, string name = null) =>
        Add("options", action, controller, routeValues, HttpMethod.Options, name);

    /// <summary>
    /// Adds a <c>trace</c> link with the TRACE method.
    /// </summary>
    /// <inheritdoc cref="Add" path="/param[@name='action' or @name='controller' or @name='routeValues' or @name='name']"/>
    /// <returns>This builder.</returns>
    public HalLinksBuilder AddTrace(string action, string controller, object routeValues = null, string name = null) =>
        Add("trace", action, controller, routeValues, HttpMethod.Trace, name);

    /// <summary>
    /// Adds a <c>connect</c> link with the CONNECT method.
    /// </summary>
    /// <inheritdoc cref="Add" path="/param[@name='action' or @name='controller' or @name='routeValues' or @name='name']"/>
    /// <returns>This builder.</returns>
    public HalLinksBuilder AddConnect(string action, string controller, object routeValues = null, string name = null) =>
        Add("connect", action, controller, routeValues, HttpMethod.Connect, name);

    /// <summary>
    /// Gets the links added so far.
    /// </summary>
    /// <returns>The links. Further Add calls on the builder are reflected in the same instance.</returns>
    public HalLinks Build() => links;
}
