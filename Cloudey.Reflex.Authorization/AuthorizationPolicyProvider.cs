using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Cloudey.Reflex.Authorization;

public class AuthorizationPolicyProvider : IAuthorizationPolicyProvider
{
    private readonly IOptions<AuthorizationOptions> _options;
    private readonly IServiceScope _scope;

    public AuthorizationPolicyProvider (IServiceScope scope, IOptions<AuthorizationOptions> options)
    {
        _scope = scope;
        _options = options;
    }

    public Task<AuthorizationPolicy?> GetPolicyAsync (string policyName)
    {
        var policies = _scope.ServiceProvider.GetKeyedServices<AuthorizationPolicy>(policyName).ToList();

        return policies.Count switch
        {
            0 => Task.FromResult<AuthorizationPolicy?>(null),
            1 => Task.FromResult<AuthorizationPolicy?>(policies.First()),
            _ => Task.FromResult<AuthorizationPolicy?>(
                policies.Aggregate((previous, next) => new AuthorizationPolicyBuilder(previous).Combine(next).Build())
            ),
        };
    }

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync () => Task.FromResult(_options.Value.DefaultPolicy);

    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync () => Task.FromResult(_options.Value.FallbackPolicy);
}