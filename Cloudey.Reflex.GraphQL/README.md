# Reflex
## GraphQL

---
Opinionated setup for a GraphQL server with the Reflex framework using HotChocolate.

- Auto-discovery of queries, mutations, and subscriptions in included assemblies (using the Reflex.Core `IncludeAssembly` attribute)
- Opinionated configuration of sorting, paging, projections, and filtering
- Support for validation with FluentValidation
- Support for Ulid types (used by Reflex.Database)
- Included error type

### Installation

Install with [NuGet](https://www.nuget.org/packages/Cloudey.Reflex.Database/)

### Usage

This library provides an opinionated setup for a GraphQL server with the Reflex framework using HotChocolate.
Enable it with the `AddReflexGraphQL` method on the HotChocolate server builder:

```c#
var builder = services.AddGraphQLServer()

// Enable queries, mutations and subscriptions
builder
    .AddQueryType()
    .AddMutationType()
    .AddSubscriptionType().AddInMemorySubscriptions(); // Remove if you have no subscriptions
    
// Enable Reflex GraphQL
builder.AddReflexGraphQL();
```

### Authorization

See [Cloudey.Reflex.Authorization.HotChocolate](https://github.com/CloudeyIT/Reflex/tree/master/Cloudey.Reflex.Authorization.HotChocolate)

### Made by
**[Cloudey](https://cloudey.com)**

### License

Licensed under Apache 2.0.  
**Copyright © 2023 Cloudey IT Ltd**  
Cloudey® is a registered trademark of Cloudey IT Ltd. Use of the trademark is NOT GRANTED under the license of this repository or software package.