using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace Cloudey.Reflex.Authorization;

public class AssertionRequirementHandler : AuthorizationHandler<AssertionRequirement>
{
	protected override async Task HandleRequirementAsync (
		AuthorizationHandlerContext context,
		AssertionRequirement requirement
	)
	{
		await requirement.HandleAsync(context);
	}
}