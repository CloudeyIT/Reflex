using Microsoft.AspNetCore.Authorization;

namespace Cloudey.Reflex.Authorization;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class Authorize : AuthorizeAttribute
{
	/// <summary>
	/// Initializes a new instance of the <see cref="AuthorizeAttribute"/> class.
	/// </summary>
	public Authorize () { }

	/// <summary>
	/// Initializes a new instance of the <see cref="AuthorizeAttribute"/> class with the specified policy.
	/// </summary>
	/// <param name="policy">The name of the policy to require for authorization.</param>
	public Authorize (string policy)
	{
		Policy = policy;
	}

	public Authorize (Type policy) : base(policy.Name) {}
	
	public Authorize (params string[] roles)
	{
		Roles = string.Join(",", roles);
	}
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class Authorize<T> : Authorize where T : IPolicy
{
	/// <summary>
	/// Applies the given policy on authorization.
	/// </summary>
	public Authorize () : base(typeof(T)) { }
}