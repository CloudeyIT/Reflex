using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Cloudey.Reflex.Authorization;

public static class DependencyInjectionExtensions
{
    public static void AddReflexAuthorization (this IServiceCollection services, params Assembly[] assemblies) =>
        AddReflexAuthorization(services, null, assemblies);

    public static void AddReflexAuthorization (
        this IServiceCollection services,
        Action<AuthorizationOptions>? configure = null,
        params Assembly[] assemblies
    )
    {
        services.AddAuthorizationCore(configure ?? (_ => { }));
        services.AddSingleton<IAuthorizationPolicyProvider, AuthorizationPolicyProvider>();
        services.AddSingleton<IAuthorizationHandlerProvider, AuthorizationHandlerProvider>();
        services.AddSingleton<AssertionRequirementHandler>();
        services.AddSingleton<AuthorizationHandler<AssertionRequirement>>(provider => provider.GetRequiredService<AssertionRequirementHandler>());
        services.AddSingleton<IAuthorizationHandler>(provider => provider.GetRequiredService<AssertionRequirementHandler>());
        services.AddSingleton<IAuthorizationService, AuthorizationService>();

        foreach (var assembly in assemblies)
        {
            var handlers = assembly.GetTypes()
                .Where(
                    t => t.IsGenericTypeDefinition &&
                         (t.GetGenericTypeDefinition() == typeof(AuthorizationHandler<>) ||
                          t.GetGenericTypeDefinition() == typeof(AuthorizationHandler<,>))
                )
                .ToArray();

            foreach (var handler in handlers) services.AddScoped(handler, typeof(IAuthorizationHandler));

            var policies = assembly.GetTypes()
                .Where(t => t.IsAssignableTo(typeof(IPolicy)) && !t.IsInterface)
                .ToArray();

            foreach (var policy in policies)
            {
                if (policy.GetProperty(nameof(IPolicy.Policy))?.GetValue(null, null)
                    is not AuthorizationPolicy definition) return;
                services.AddKeyedSingleton(policy.FullName, definition);
            }
        }
    }
}