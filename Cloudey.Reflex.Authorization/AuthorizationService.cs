using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Cloudey.Reflex.Authorization;

public class AuthorizationService : IAuthorizationService
{
    private readonly IAuthorizationHandlerContextFactory _contextFactory;
    private readonly IAuthorizationEvaluator _evaluator;
    private readonly IAuthorizationHandlerProvider _handlerProvider;
    private readonly IAuthorizationPolicyProvider _policyProvider;

    public AuthorizationService (
        IAuthorizationHandlerContextFactory contextFactory,
        IAuthorizationHandlerProvider handlerProvider,
        IAuthorizationEvaluator evaluator,
        IAuthorizationPolicyProvider policyProvider
    )
    {
        _contextFactory = contextFactory;
        _handlerProvider = handlerProvider;
        _evaluator = evaluator;
        _policyProvider = policyProvider;
    }

    public async Task<AuthorizationResult> AuthorizeAsync (
        ClaimsPrincipal user,
        object? resource,
        IEnumerable<IAuthorizationRequirement> requirements
    )
    {
        var authContext = _contextFactory.CreateContext(requirements, user, resource);

        var handlers = await _handlerProvider.GetHandlersAsync(authContext);

        var handlerTasks = handlers.Select(handler => handler.HandleAsync(authContext));

        await Task.WhenAll(handlerTasks);

        return _evaluator.Evaluate(authContext);
    }

    public async Task<AuthorizationResult> AuthorizeAsync (
        ClaimsPrincipal user,
        object? resource,
        string policyName
    )
    {
        var policy = await _policyProvider.GetPolicyAsync(policyName);

        if (policy is null) throw new ArgumentException($"Policy {policyName} not found");

        var authContext = _contextFactory.CreateContext(policy.Requirements, user, resource);
        var handlers = await _handlerProvider.GetHandlersAsync(authContext);

        var handlerTasks = handlers.Select(handler => handler.HandleAsync(authContext));

        await Task.WhenAll(handlerTasks);

        return _evaluator.Evaluate(authContext);
    }
}