using Microsoft.AspNetCore.Http;

namespace Dzaba.Utils.AspNet.Hal;

/// <summary>
/// Builds HAL links to controller actions.
/// </summary>
public interface ILinkFactory
{
    /// <summary>
    /// Builds a link to the given controller action.
    /// </summary>
    /// <param name="httpContext">The current HTTP context.</param>
    /// <param name="action">The name of the action.</param>
    /// <param name="controller">The name of the controller.</param>
    /// <param name="routeValues">Optional route values used to generate the URL.</param>
    /// <param name="method">The optional HTTP method to advertise in the link.</param>
    /// <returns>The built link.</returns>
    HalLink Build(HttpContext httpContext, string action, string controller, object routeValues = null, string method = null);
}
