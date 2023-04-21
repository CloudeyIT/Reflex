# Reflex
## Authorization

---

**Plug-and-play authorization extending ASP.NET Core Authorization to make it easier and more pleasant to use.**  
Most features require Autofac.

### Features  

- Plug-and-play, no boilerplate required
- Provides authorization services and providers for use with Autofac DI
- Define authorization policies without boilerplate
- Policies are auto-registered with the DI container
- Reference policies by type instead of name for better type safety

### Installation

Install with [NuGet](https://www.nuget.org/packages/Cloudey.Reflex.Authorization/)

Register the authorization services with Autofac:
```c#
using Autofac;

public class MyModule : Module
{
    protected override void Load (ContainerBuilder builder)
    {
        builder.RegisterReflexAuthorization(ThisAssembly); // Provide a list of assemblies to scan for policies and authorization handlers
    }
}
```

If you are not using ASP.NET Core Authorization already, register the authorization middleware:
```c#
// Program.cs

var builder = WebApplication.CreateBuilder(args);
// ...
var app = builder.Build();
// ...
app.UseAuthorization(); // <-- Register authorization middleware
// ...

await app.RunAsync();
```

### Usage

#### Policies

Define policies anywhere in your application by implementing the `IPolicy` interface:

```c#
public class MustBeAdminPolicy : IPolicy
{
	public static AuthorizationPolicy Policy { get; } = new AuthorizationPolicyBuilder()
		.RequireAuthenticatedUser()
		.RequireRole("Admin")
		.Build();
}
```

Use the policy with the extended `Authorize` attribute:

```c#
using Cloudey.Reflex.Authorization; // <-- Use the Authorize attribute from this library

// ...

[Authorize<MustBeAdminPolicy>] // <-- Use the policy type instead of the policy name
[HttpGet]
public void AdminOnlyEndpoint()
{
    // ...
}
```

Alternatively, you can reference the policy by its name:

```c#
using Microsoft.AspNetCore.Authorization; // <-- In this case, using the default ASP.NET Core Authorize attribute also works

[Authorize(nameof(MustBeAdminPolicy))] // <-- Use the policy type instead of the policy name
[HttpGet]
public void AdminOnlyEndpoint()
{
    // ...
}
```

That's it! There is no need to register the policy anywhere else or define additional handlers.

#### Assertions

Assertions are easy ways to add more complex requirements to your policies.

```c#
public class UserPolicy : IPolicy
{
	public static AuthorizationPolicy Policy { get; } = new AuthorizationPolicyBuilder()
		.RequireAuthenticatedUser()
		.RequireAssertion(context => context.User.HasClaim("superuser", "true"))
		.Build();
}
```

There is no further configuration required to handle assertion requirements.

### GraphQL / HotChocolate

If you are using HotChocolate for GraphQL, check out [Cloudey.Reflex.Authorization.HotChocolate](https://www.nuget.org/packages/Cloudey.Reflex.Authorization.HotChocolate/) for a plug-and-play solution to authorization in GraphQL.

### License

Licensed under Apache 2.0.  
**Copyright © 2023 Cloudey IT Ltd**  
Cloudey® is a registered trademark of Cloudey IT Ltd. Use of the trademark is NOT GRANTED under the license of this repository or software package.