using Microsoft.AspNetCore.Authorization;

namespace Cloudey.Reflex.Authorization;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class Authorize : AuthorizeAttribute
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="AuthorizeAttribute" /> class.
    /// </summary>
    public Authorize () { }

    public Authorize (Type policy) : base(policy.FullName!) { }

    public Authorize (params string[] roles)
    {
        Roles = string.Join(",", roles);
    }
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class Authorize<T> : Authorize where T : IPolicy
{
    /// <summary>
    ///     Applies the given policy on authorization.
    /// </summary>
    public Authorize () : base(typeof(T)) { }
}