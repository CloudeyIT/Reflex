# Reflex
## Authorization for HotChocolate

---

**Utilities for easy-to-use authorization with HotChocolate GraphQL server.**  


### Features  

- Assertions for types, fields, and parent types
- Easy policy-based authorization
- Works with [Cloudey.Reflex.Authorization](https://www.nuget.org/packages/Cloudey.Reflex.Authorization/)

### Installation

Install with [NuGet](https://www.nuget.org/packages/Cloudey.Reflex.Authorization.HotChocolate/)

Make sure authorization is enabled for your HotChocolate server:
```c#
services.AddGraphQLServer()
    // ...
    .AddAuthorization() // <-- Enable authorization
    // ...

```

### Using the GraphQL assertions

#### Parent assertion

`RequireParentAssertion` enables you to assert that the _parent_ of a field fulfils a condition.


**NOTE**
When using projection, non-projected fields will _not_ be resolved in the delegate. Therefore, it is important that you mark any other fields that you need in your delegate with the `[IsProjected]` attribute to make sure they are always loaded, otherwise they will be `null` in the assertion delegate.

The delegate receives the following arguments:
- The _parent_ of type `T`
- The authorization context `AuthorizationHandlerContext`
- The GraphQL context `IMiddlewareContext`

Example:
```c#
// UserPolicy.cs
// IMPORTANT: Defining policies in this way requires setting up Cloudey.Reflex.Authorization
public class SecretKeyPolicy : IPolicy
{
    public static AuthorizationPolicy Policy { get; } = new AuthorizationPolicyBuilder()
        .RequireParentAssertion<User>(
        // The secret key can only be accessed by the user itself or an admin
            (user, context, directiveContext) => user.Id == context.User.GetId() || context.User.IsInRole(Role.Admin)
        )
        .Build();
}

// User.cs

public class User : Entity {
    // ...
    [Guard<SecretKeyPolicy>]
    public string SecretKey { get; set; }
}
```

#### Field assertion

`RequireResultAssertion` enables you to assert that the resolved entity or field fulfills a condition.

**Important**  
If the policy is applied to a field, the target is the value of the field.  
If the policy is applied to a class, the target is the class instance.  
If the policy is applied to a resolver, the target is the result of the resolver.  
If the result is an IEnumerable, then the assertion is applied to all elements.

The delegate receives the following arguments:
- The _field value_ `T`
- The authorization context `AuthorizationHandlerContext`
- The GraphQL context `IMiddlewareContext`

Example:
```c#
// UserPolicy.cs
// IMPORTANT: Defining policies in this way requires setting up Cloudey.Reflex.Authorization
public class AvatarPolicy : IPolicy
{
    public static AuthorizationPolicy Policy { get; } = new AuthorizationPolicyBuilder()
        .RequireTargetAssertion<Avatar>(
        // The avatar can only be accessed if it is set as public
            (avatar, context, directiveContext) => avatar.IsPublic
        )
        .Build();
}

// User.cs

public class User : Entity {
    // ...
    [Guard<AvatarPolicy>] // Can be applied here to only have an effect when accessed through User
    public Avatar? Avatar { get; set; }
}

// Avatar.cs

[Guard<AvatarPolicy>] // Can also be applied here to always have an effect whenever Avatar is resolved, incl. through other types
public class Avatar : Entity {
    // ...
    public bool IsPublic { get; set; } 
}
```

#### Related assertion

`RequireRelatedAssertion` enables you to assert that the field or the entity containing the field fulfills a condition. This is useful for defining policies used on both fields, classes, and resolvers interchangeably.

**Important**  
If the policy is applied to a field with type T, the target is the value of the field.  
If the policy is applied to a member of T which is not of type T, the target is an instance of the parent T.  
If the policy is applied to a class of type T, the target is the instance of the class.  
If the policy is applied to a resolver of return type T, the result is the result of the resolver.  
If the result is an IEnumerable, then the assertion is applied to all elements.

**NOTE**
When using projection, non-projected fields will _not_ be resolved in the delegate. Therefore, it is important that you mark any other fields that you need in your delegate with the `[IsProjected]` attribute to make sure they are always loaded, otherwise they will be `null` in the resolver if the field is not requested.

The delegate receives the following arguments:
- The _related type_ `T`
- The authorization context `AuthorizationHandlerContext`
- The GraphQL context `IMiddlewareContext`

Example:
```c#
// UserPolicy.cs
// IMPORTANT: Defining policies in this way requires setting up Cloudey.Reflex.Authorization
public class AvatarPolicy : IPolicy
{
    public static AuthorizationPolicy Policy { get; } = new AuthorizationPolicyBuilder()
        .RequireRelatedAssertion<Avatar>(
        // The avatar can only be accessed if it is set as public
            (avatar, context, directiveContext) => avatar.IsPublic
        )
        .Build();
}

// User.cs

public class User : Entity {
    // ...
    [Guard<AvatarPolicy>] // Here, the assertion is applied to Avatar
    public Avatar? Avatar { get; set; }
}

// Avatar.cs

[Guard<AvatarPolicy>] // Here, the assertion is applied to Avatar
public class Avatar : Entity {
    // ...
    [Guard<AvatarPolicy>] // Here, the assertion is applied to Avatar (the parent)
    public bool IsPublic { get; set; } 
    
    [Guard<AvatarPolicy>] // Here, the assertion is applied to the FIELD of type Avatar (the field not the parent!)
    public Avatar AlternativeAvatar { get; set; } // Just an example
}
```

### Authorization in general

To authenticate a given request, entity, or property, use the `[Guard]` attribute. The Guard attribute can be used to authenticate based on roles or a policy, and replaces the `Authorize` attribute from HotChocolate.

### Authorizing a query or mutation

Apply the `[Guard]` attribute to the query or mutation **method**, eg:
```c#
[QueryType]
public class MyQuery {
    ...
    [Guard(new[] { "Admin" })] // Only users with the "Admin" role can access this query
    public async string GetHello () {
        return "Hello";
    }
}
```

### Authorizing an entity

Apply the `[Guard]` attribute to the entity class, eg:
```c#
[Guard(new[] { "Admin" })]
public class SecretInformation : Entity {
    ...
}
```
This prevents anyone without the `Admin` role from accessing the entity in any query.

### Authorizing a property

You can also restrict access on a field-level. Apply the `[Guard]` attribute to the field, eg:
```c#
public class User : Entity {
    public Guid Id {get; set;}

    [Guard(new[] { "Admin" })]
    public string PasswordHash {get; set;}
}
```
This disallows access to the PasswordHash fields for everyone except Admins.

### Combining the authorization attributes

You can apply authorization attributes on multiple `levels`, and they will all be executed in order. E.g. you can allow access to the Role entity to all authenticated users with a `[Guard]` attribute on the Role class, but only allow access for Admins to a specific field in that entity by adding `[Guard(new[] { "Admin" })]` to that field.

### Using policies

When simple role-based authentication is not enough, you can also use policies to create more complex authorization logic. For an easy way to implement policy-based authorization, see [Cloudey.Reflex.Authorization](https://www.nuget.org/packages/Cloudey.Reflex.Authorization/).

### License

Licensed under Apache 2.0.  
**Copyright © 2023 Cloudey IT Ltd**  
Cloudey® is a registered trademark of Cloudey IT Ltd. Use of the trademark is NOT GRANTED under the license of this repository or software package.