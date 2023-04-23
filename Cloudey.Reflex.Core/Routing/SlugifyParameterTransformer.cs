using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Cloudey.Reflex.Core.Routing;

public class SlugifyParameterTransformer : IOutboundParameterTransformer
{
	public string? TransformOutbound (object? value)
	{
		return value == null ? null : Regex.Replace(value!.ToString()!, "([a-z])([A-Z])", "$1-$2").ToLower();
	}
}

public static class SlugifyExtensions
{
	public static IMvcBuilder UseSlugRoutes (this IMvcBuilder builder)
	{
		return builder.AddMvcOptions(
			options =>
			{
				options.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer()));
			}
		);
	}
}