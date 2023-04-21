using Autofac;
using Microsoft.AspNetCore.Authorization;

namespace Cloudey.Reflex.Authorization;

public class AuthorizationPolicyProvider : IAuthorizationPolicyProvider
{
	private readonly ILifetimeScope _scope;

	public AuthorizationPolicyProvider (ILifetimeScope scope)
	{
		_scope = scope;
	}

	public Task<AuthorizationPolicy?> GetPolicyAsync (string policyName)
	{
		return Task.FromResult(_scope.ResolveOptionalNamed<AuthorizationPolicy>(policyName));
	}

	public Task<AuthorizationPolicy> GetDefaultPolicyAsync ()
	{
		return Task.FromResult(
			new AuthorizationPolicyBuilder()
				.RequireAuthenticatedUser()
				.Build()
		);
	}

	public Task<AuthorizationPolicy?> GetFallbackPolicyAsync ()
	{
		return Task.FromResult<AuthorizationPolicy?>(null);
	}
}