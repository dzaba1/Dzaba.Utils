# Dzaba.AspNetUtils

Utilities for ASP.NET development.

## Usage

### Action filters

Namespace: `Dzaba.AspNetUtils.ActionFilters`. Both filters can be applied to a controller or a single action.

- `[ValidateModel]` returns `400 Bad Request` with the model state when it is invalid, so you don't need to check `ModelState.IsValid` in every action.
- `[HandleErrors]` turns two exceptions into HTTP responses (and logs them as warnings):
  - `ModelStateException` becomes `400 Bad Request` with the errors as model state.
  - `HttpResponseException` becomes a response with the given status code and the exception message as the body.

```csharp
using Dzaba.AspNetUtils;
using Dzaba.AspNetUtils.ActionFilters;

[ApiController]
[Route("api/[controller]")]
[HandleErrors]
public class UsersController : ControllerBase
{
    [HttpPost]
    [ValidateModel]
    public IActionResult Create(CreateUserRequest request)
    {
        if (users.Exists(request.Email))
        {
            throw new ModelStateException(
            [
                new KeyValuePair<string, string>(nameof(request.Email), "Email is already taken.")
            ]);
        }

        return Ok();
    }

    [HttpGet("{id}")]
    public IActionResult Get(int id)
    {
        var user = users.Find(id)
            ?? throw new HttpResponseException(HttpStatusCode.NotFound, $"User {id} not found.");

        return Ok(user);
    }
}
```

### Hal

Namespace: `Dzaba.Utils.AspNet.Hal`. Helpers for building [HAL](https://datatracker.ietf.org/doc/html/draft-kelly-json-hal) responses. Register the link factory first:

```csharp
builder.Services.AddDzabaAspNetServices();
```

Derive your controller from `HalController`. It takes the `ILinkFactory` and the MVC `JsonOptions` (inject `IOptions<JsonOptions>` and pass its `Value`) and gives you:

- `CreateLinksBuilder()` - a `HalLinksBuilder` for the current request. It has `AddSelf`, `AddGet`, `AddPost`, `AddPut`, `AddPatch`, `AddDelete`, `AddHead`, `AddOptions`, `AddTrace`, `AddConnect` (each with `action, controller, routeValues = null, name = null`) and `Add(rel, ...)` for custom relations. `Build()` returns the `HalLinks`.
- `OkHal(resource, links)` and `OkHal(embeddedKey, items, links, itemLinks = null)` - `200 OK` with a HAL resource or collection.

```csharp
[ApiController]
[Route("api/[controller]")]
public class UsersController(ILinkFactory links, IOptions<JsonOptions> jsonOptions)
    : HalController(links, jsonOptions.Value)
{
    [HttpGet("{id}")]
    public IActionResult Get(int id)
    {
        var user = users.Find(id);

        var userLinks = CreateLinksBuilder()
            .AddSelf(nameof(Get), "Users", new { id })
            .AddDelete(nameof(Delete), "Users", new { id })
            .Build();

        return OkHal(user, userLinks);
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var collectionLinks = CreateLinksBuilder()
            .AddSelf(nameof(GetAll), "Users")
            .Build();

        return OkHal("users", users.All(), collectionLinks,
            user => CreateLinksBuilder()
                .AddSelf(nameof(Get), "Users", new { id = user.Id })
                .Build());
    }
}
```

`Get` returns:

```json
{
  "id": 1,
  "name": "Jane",
  "_links": {
    "self": { "href": "/api/Users/1", "method": null },
    "delete": { "href": "/api/Users/1", "method": "DELETE" }
  }
}
```

`GetAll` returns `_links` and `_embedded.users` (an array of the items, each with its own `_links`).

Calling the same `AddX` method again adds another link to that relation, which is then serialized as an array. Use `name` to tell such links apart. A relation with a single link is always a single object.

### JWT Auth

Namespace: `Dzaba.Utils.AspNet` (extensions are in `Dzaba.AspNetUtils`).

Configure JWT bearer authentication from a `JwtSettings` object:

```csharp
var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>();

builder.Services.AddJwtSettings(jwtSettings);
builder.Services.AddJwtAuthentication(() => jwtSettings);
```

```json
{
  "Jwt": {
    "Authority": "https://auth.example.com",
    "Audience": "my-api",
    "RequireHttps": true,
    "ValidateAudience": true,
    "ValidateIssuer": true,
    "IssuerSigningKey": "a-long-secret-key-of-at-least-32-chars"
  }
}
```

`IssuerSigningKey` is optional. When set, tokens are validated with this symmetric key.

Create tokens with `JwtTokenBuilder`:

```csharp
var token = new JwtTokenBuilder()
    .WithSecurityKey(jwtSettings.IssuerSigningKey)
    .WithIssuer(jwtSettings.Authority)
    .WithAudience(jwtSettings.Audience)
    .WithSubject(user.Id)
    .WithName(user.Name)
    .WithClaim("role", "admin")
    .WithLifetime(TimeSpan.FromHours(1))
    .Build()
    .EncodeToString();
```

Read the current user's id (the `sub` claim, or the name identifier as a fallback):

```csharp
var userId = HttpContext.GetUserSubOrNameIdentifier();
// or
var userId = User.GetUserSubOrNameIdentifier();
```
