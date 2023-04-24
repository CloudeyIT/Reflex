using HotChocolate.Authorization;

namespace Cloudey.Reflex.Authorization.HotChocolate;

[AttributeUsage(
	AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Property | AttributeTargets.Method,
	AllowMultiple = true
)]
public class Guard : AuthorizeAttribute
{
	/// <summary>
	///     Applies the authorization directive to object types or object fields.
	/// </summary>
	public Guard () { }

	/// <summary>
	///     Applies the authorization directive with a specific policy to
	///     object types or object fields.
	/// </summary>
	public Guard (string policy) : base(policy, ApplyPolicy.AfterResolver)
	{
		Policy = policy;
		Apply = ApplyPolicy.AfterResolver;
	}

	/// <summary>
	///     Applies the authorization directive with a specific policy to
	///     object types or object fields.
	/// </summary>
	public Guard (string policy, ApplyPolicy apply = ApplyPolicy.AfterResolver) : base(policy, apply)
	{
		Policy = policy;
		Apply = apply;
	}

	public Guard (Type policy) : base(policy.Name, ApplyPolicy.AfterResolver) { }

	public Guard (Type policy, ApplyPolicy apply = ApplyPolicy.AfterResolver) : base(policy.Name, apply) { }

	public Guard (string[] roles)
	{
		Roles = roles;
		Apply = ApplyPolicy.BeforeResolver;
	}
}

public class Guard<T> : Guard where T : IPolicy
{
	public Guard () : base(typeof(T)) { }

	public Guard (ApplyPolicy apply = ApplyPolicy.AfterResolver) : base(typeof(T), apply) { }
}