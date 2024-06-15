using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Cloudey.Reflex.Authorization;

public class AuthorizationHandlerProvider : IAuthorizationHandlerProvider
{
    private readonly IServiceScope _scope;

    public AuthorizationHandlerProvider (IServiceScope scope)
    {
        _scope = scope;
    }

    public Task<IEnumerable<IAuthorizationHandler>> GetHandlersAsync (AuthorizationHandlerContext context)
    {
        var handlers = new List<IAuthorizationHandler>();

        foreach (var requirement in context.Requirements)
            if (requirement.GetType()
                    .IsAssignableTo(typeof(AuthorizationHandler<>).MakeGenericType(requirement.GetType())) ||
                (context.Resource is not null &&
                 requirement.GetType()
                     .IsAssignableTo(
                         typeof(AuthorizationHandler<,>).MakeGenericType(
                             requirement.GetType(),
                             context.Resource.GetType()
                         )
                     )))
            {
                handlers.Add((requirement as IAuthorizationHandler)!);
            }
            else if (requirement is AssertionRequirement)
            {
                handlers.AddRange(_scope.ServiceProvider.GetServices<AuthorizationHandler<AssertionRequirement>>());
            }
            else
            {
                handlers.AddRange(ResolveGeneralHandlers(requirement));
                handlers.AddRange(ResolveResourceHandlers(requirement, context));
            }

        return Task.FromResult(handlers.AsEnumerable());
    }

    private IEnumerable<IAuthorizationHandler> ResolveGeneralHandlers (IAuthorizationRequirement requirement) =>
        _scope.ServiceProvider.GetServices(typeof(AuthorizationHandler<>).MakeGenericType(requirement.GetType()))
            as IEnumerable<IAuthorizationHandler> ??
        Array.Empty<IAuthorizationHandler>();

    private IEnumerable<IAuthorizationHandler> ResolveResourceHandlers (
        IAuthorizationRequirement requirement,
        AuthorizationHandlerContext context
    )
    {
        if (context.Resource is null) return new List<IAuthorizationHandler>();

        var handlers = new List<IAuthorizationHandler>();
        var concreteHandlers = _scope.ServiceProvider.GetServices(
            typeof(AuthorizationHandler<,>).MakeGenericType(
                requirement.GetType(),
                context.Resource!.GetType()
            )
        ) as IEnumerable<IAuthorizationHandler>;

        var interfaceHandlers = context.Resource!.GetType()
            .GetInterfaces()
            .SelectMany(
                handledInterface => _scope.ServiceProvider.GetServices(
                                        typeof(AuthorizationHandler<,>).MakeGenericType(
                                            requirement.GetType(),
                                            handledInterface
                                        )
                                    ) as IEnumerable<IAuthorizationHandler> ??
                                    Array.Empty<IAuthorizationHandler>()
            );

        if (concreteHandlers is not null) handlers.AddRange(concreteHandlers);

        handlers.AddRange(interfaceHandlers);

        return handlers;
    }
}