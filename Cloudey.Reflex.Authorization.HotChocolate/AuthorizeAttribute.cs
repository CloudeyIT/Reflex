using HotChocolate.Authorization;

namespace Cloudey.Reflex.Authorization.HotChocolate;

public class AuthorizeAttribute<T> : AuthorizeAttribute where T : IPolicy
{
	public AuthorizeAttribute () : base(typeof(T).FullName!) { }

	public AuthorizeAttribute (ApplyPolicy apply = ApplyPolicy.AfterResolver) : base(typeof(T).FullName!, apply) { }
}