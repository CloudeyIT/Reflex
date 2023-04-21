using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Cloudey.Reflex.Authorization;

public static class DependencyInjectionExtensions
{
	public static void AddReflexAuthorization (this IServiceCollection services, Action<AuthorizationOptions>? configure = null)
	{
		services.AddAuthorizationCore(configure ?? (_ => {}));
		services.AddSingleton<IAuthorizationPolicyProvider, AuthorizationPolicyProvider>();
		services.AddSingleton<IAuthorizationHandlerProvider, AuthorizationHandlerProvider>();
		services.AddSingleton<IAuthorizationHandler, AssertionRequirementHandler>();
		services.AddSingleton<IAuthorizationService, AuthorizationService>();
	}
}