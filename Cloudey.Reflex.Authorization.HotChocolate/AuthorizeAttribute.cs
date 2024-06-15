using HotChocolate.Authorization;

namespace Cloudey.Reflex.Authorization.HotChocolate;

public class GuardAttribute<T> : AuthorizeAttribute where T : IPolicy
{
	public GuardAttribute () : base(typeof(T).FullName!) { }

	public GuardAttribute (ApplyPolicy apply = ApplyPolicy.AfterResolver) : base(typeof(T).FullName!, apply) { }
}

public class GuardAttribute : AuthorizeAttribute
{
    public GuardAttribute () { }

    public GuardAttribute (ApplyPolicy apply = ApplyPolicy.AfterResolver) : base(apply) { }

    public GuardAttribute (params string[] roles)
    {
        Roles = roles;
    }
}