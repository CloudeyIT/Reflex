using System.Net;
using HotChocolate.AspNetCore.Serialization;
using HotChocolate.Execution;

namespace Cloudey.Reflex.GraphQL.Middleware;

public class CustomHttpResultFormatter : DefaultHttpResponseFormatter
{
	protected override HttpStatusCode OnDetermineStatusCode (
		IOperationResult result,
		FormatInfo format,
		HttpStatusCode? proposedStatusCode
	)
	{
		if (result.Errors?.Any(
			    e => e.Extensions?.TryGetValue("code", out var code) is true && code is "AUTH_NOT_AUTHORIZED"
		    ) ??
		    false)
		{
			return HttpStatusCode.Unauthorized;
		}
		
		if (result.Errors?.All(
			    e => e.Extensions?.TryGetValue("code", out var code) is true && code is "FairyBread_ValidationError"
		    ) ??
		    false)
		{
			return HttpStatusCode.OK;
		}

		return base.OnDetermineStatusCode(result, format, proposedStatusCode);
	}
}