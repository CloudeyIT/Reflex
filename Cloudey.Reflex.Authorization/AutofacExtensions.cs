using System.Reflection;
using Autofac;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace Cloudey.Reflex.Authorization;

public static class AutofacExtensions
{
	public static void RegisterReflexAuthorization (this ContainerBuilder builder, params Assembly[] assemblies)
	{
		builder.RegisterType<AuthorizationPolicyProvider>()
			.As<IAuthorizationPolicyProvider>()
			.SingleInstance();

		builder.RegisterType<AuthorizationHandlerProvider>()
			.As<IAuthorizationHandlerProvider>()
			.SingleInstance();

		builder.RegisterType<AuthorizationService>()
			.As<IAuthorizationService>()
			.SingleInstance();

		builder.RegisterType<AssertionRequirementHandler>()
			.As<AuthorizationHandler<AssertionRequirement>>()
			.SingleInstance();

		foreach (var assembly in assemblies)
		{
			builder.RegisterAssemblyTypes(assembly)
				.AsClosedTypesOf(typeof(AuthorizationHandler<>))
				.As<IAuthorizationHandler>()
				.InstancePerLifetimeScope();

			builder.RegisterAssemblyTypes(assembly)
				.AsClosedTypesOf(typeof(AuthorizationHandler<,>))
				.As<IAuthorizationHandler>()
				.InstancePerLifetimeScope();

			foreach (var policy in assembly.GetTypes()
				         .Where(type => type.IsAssignableTo(typeof(IPolicy)) && !type.IsInterface))
			{
				if (policy.GetProperty("Policy")?.GetValue(null, null) is not AuthorizationPolicy authorizationPolicy)
					return;

				builder.RegisterInstance(authorizationPolicy)
					.Named<AuthorizationPolicy>(policy.Name)
					.As<AuthorizationPolicy>()
					.SingleInstance();
			}
		}
	}
}