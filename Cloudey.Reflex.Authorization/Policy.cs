using Microsoft.AspNetCore.Authorization;

namespace Cloudey.Reflex.Authorization;

public interface IPolicy
{
    public static abstract AuthorizationPolicy Policy { get; }
}