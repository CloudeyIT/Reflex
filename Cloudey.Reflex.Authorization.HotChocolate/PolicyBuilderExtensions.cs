﻿using HotChocolate.Resolvers;
using Microsoft.AspNetCore.Authorization;

namespace Cloudey.Reflex.Authorization.HotChocolate;

public static class PolicyBuilderExtensions
{
	/// <summary>
	///     Require the parent object to fulfil the given assertion.
	///     If the policy is applied to a field, the parent is the class that contains the field.
	/// </summary>
	/// <param name="builder"></param>
	/// <param name="expression"></param>
	/// <typeparam name="T">Type of the parent</typeparam>
	/// <returns></returns>
	public static AuthorizationPolicyBuilder RequireParentAssertion<T> (
		this AuthorizationPolicyBuilder builder,
		Func<T, AuthorizationHandlerContext, IResolverContext, bool> expression
	)
	{
		return builder.RequireAssertion(
			context =>
			{
				switch (context.Resource)
				{
					case IMiddlewareContext middlewareContext:
					{
						var parent = middlewareContext.Parent<T>();
						return expression.Invoke(parent, context, middlewareContext);
					}
					default:
						throw new ApplicationException(
							"Invalid target for parent assertion requirement. Must be applied to a field in a GraphQL context!"
						);
				}
			}
		);
	}

	/// <summary>
	///     Require the target object to fulfil the given assertion.
	///     If the policy is applied to a field, the result is the value of the field.
	///     If the policy is applied to a class, the result is the class instance.
	///     If the policy is applied to a resolver, the result is the result of the resolver.
	///     If the result is an IEnumerable, then the assertion is applied to all elements.
	/// </summary>
	/// <param name="builder"></param>
	/// <param name="expression"></param>
	/// <typeparam name="T">Type of the parent</typeparam>
	/// <returns></returns>
	public static AuthorizationPolicyBuilder RequireTargetAssertion<T> (
		this AuthorizationPolicyBuilder builder,
		Func<T, AuthorizationHandlerContext, IMiddlewareContext, bool> expression
	)

	{
		return builder.RequireAssertion(
			context =>
			{
				switch (context.Resource)
				{
					case IMiddlewareContext middlewareContext:
					{
						var result = middlewareContext.Result;

						return result switch
						{
							T singleResult => expression.Invoke(singleResult, context, middlewareContext),
							IEnumerable<T> manyResults => manyResults.All(
								x => expression.Invoke(x, context, middlewareContext)
							),
							_ => throw new ApplicationException(
								"Invalid result type for result assertion requirement. Must be the same as the type of the target!"
							),
						};
					}
					default:
						throw new ApplicationException(
							"Invalid target for result assertion requirement. Must be applied to a class or field in a GraphQL context and applied with AFTER_RESOLVER!"
						);
				}
			}
		);
	}

	/// <summary>
	///     Require the related entity of the target to fulfil the given assertion.
	///     If the policy is applied to a field with type T, the target is the value of the field.
	///     If the policy is applied to a member of T which is not of type T, the target is an instance of the parent T.
	///     If the policy is applied to a class of type T, the target is the instance of the class.
	///     If the policy is applied to a resolver of return type T, the result is the result of the resolver.
	///     If the result is an IEnumerable, then the assertion is applied to all elements.
	/// </summary>
	/// <param name="builder"></param>
	/// <param name="expression"></param>
	/// <typeparam name="T">Type of the parent</typeparam>
	/// <returns></returns>
	public static AuthorizationPolicyBuilder RequireRelatedAssertion<T> (
		this AuthorizationPolicyBuilder builder,
		Func<T, AuthorizationHandlerContext, IMiddlewareContext, bool> expression
	)

	{
		return builder.RequireAssertion(
			context =>
			{
				switch (context.Resource)
				{
					case IMiddlewareContext middlewareContext:
					{
						var result = middlewareContext.Result;

						return result switch
						{
							T singleResult => expression.Invoke(singleResult, context, middlewareContext),
							IEnumerable<T> manyResults => manyResults.All(
								x => expression.Invoke(x, context, middlewareContext)
							),
							_ => middlewareContext.Parent<object?>() switch
							{
								T parent => expression.Invoke(parent, context, middlewareContext),
								_ => throw new ApplicationException(
									"Invalid result type for related assertion requirement. Must be the same as the type of the target, or on a field of the type of the target!"
								),
							},
						};
					}
					default:
						throw new ApplicationException(
							"Invalid target for result assertion requirement. Must be applied to a class or field in a GraphQL context and applied with AFTER_RESOLVER!"
						);
				}
			}
		);
	}
	
	/// <summary>
	///     Require the parent object to fulfil the given assertion.
	///     If the policy is applied to a field, the target is the class that contains the field.
	/// </summary>
	/// <param name="builder"></param>
	/// <param name="expression"></param>
	/// <typeparam name="T">Type of the target</typeparam>
	/// <returns></returns>
	public static AuthorizationPolicyBuilder RequireParentAssertion<T> (
		this AuthorizationPolicyBuilder builder,
		Func<T, AuthorizationHandlerContext, IResolverContext, Task<bool>> expression
	)
	{
		return builder.RequireAssertion(
			async context =>
			{
				switch (context.Resource)
				{
					case IMiddlewareContext middlewareContext:
					{
						var parent = middlewareContext.Parent<T>();
						return await expression.Invoke(parent, context, middlewareContext);
					}
					default:
						throw new ApplicationException(
							"Invalid target for parent assertion requirement. Must be applied to a field in a GraphQL context!"
						);
				}
			}
		);
	}

	/// <summary>
	///     Require the target object to fulfil the given assertion.
	///     If the policy is applied to a field, the target is the value of the field.
	///     If the policy is applied to a class, the target is the class instance.
	///     If the policy is applied to a resolver, the target is the result of the resolver.
	///     If the result is an IEnumerable, then the assertion is applied to all elements.
	/// </summary>
	/// <param name="builder"></param>
	/// <param name="expression"></param>
	/// <typeparam name="T">Type of the target</typeparam>
	/// <returns></returns>
	public static AuthorizationPolicyBuilder RequireTargetAssertion<T> (
		this AuthorizationPolicyBuilder builder,
		Func<T, AuthorizationHandlerContext, IMiddlewareContext, Task<bool>> expression
	)

	{
		return builder.RequireAssertion(
			async context =>
			{
				switch (context.Resource)
				{
					case IMiddlewareContext middlewareContext:
					{
						var result = middlewareContext.Result;

						return result switch
						{
							T singleResult => await expression.Invoke(singleResult, context, middlewareContext),
							IEnumerable<T> manyResults => (await Task.WhenAll(
								manyResults.Select(
									x => expression.Invoke(x, context, middlewareContext)
								)
							)).All(x => x),
							_ => throw new ApplicationException(
								"Invalid result type for result assertion requirement. Must be the same as the type of the target!"
							),
						};
					}
					default:
						throw new ApplicationException(
							"Invalid target for result assertion requirement. Must be applied to a class or field in a GraphQL context and applied with AFTER_RESOLVER!"
						);
				}
			}
		);
	}

	/// <summary>
	///     Require the related entity of the target to fulfil the given assertion.
	///     If the policy is applied to a field with type T, the target is the value of the field.
	///     If the policy is applied to a member of T which is not of type T, the target is an instance of the parent T.
	///     If the policy is applied to a class of type T, the target is the instance of the class.
	///     If the policy is applied to a resolver of return type T, the result is the result of the resolver.
	///     If the result is an IEnumerable, then the assertion is applied to all elements.
	/// </summary>
	/// <param name="builder"></param>
	/// <param name="expression"></param>
	/// <typeparam name="T">Type of the target</typeparam>
	/// <returns></returns>
	public static AuthorizationPolicyBuilder RequireRelatedAssertion<T> (
		this AuthorizationPolicyBuilder builder,
		Func<T, AuthorizationHandlerContext, IMiddlewareContext, Task<bool>> expression
	)

	{
		return builder.RequireAssertion(
			async context =>
			{
				switch (context.Resource)
				{
					case IMiddlewareContext middlewareContext:
					{
						var result = middlewareContext.Result;

						return result switch
						{
							T singleResult => await expression.Invoke(singleResult, context, middlewareContext),
							IEnumerable<T> manyResults => (await Task.WhenAll(
								manyResults.Select(
									x => expression.Invoke(x, context, middlewareContext)
								)
							)).All(x => x),
							_ => middlewareContext.Parent<object?>() switch
							{
								T parent => await expression.Invoke(parent, context, middlewareContext),
								_ => throw new ApplicationException(
									"Invalid result type for related assertion requirement. Must be the same as the type of the target, or on a field of the type of the target!"
								),
							},
						};
					}
					default:
						throw new ApplicationException(
							"Invalid target for result assertion requirement. Must be applied to a class or field in a GraphQL context and applied with AFTER_RESOLVER!"
						);
				}
			}
		);
	}
}