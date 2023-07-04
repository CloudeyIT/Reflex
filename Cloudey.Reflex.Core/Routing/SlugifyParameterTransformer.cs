using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Cloudey.Reflex.Core.Routing;

public partial class SlugifyParameterTransformer : IOutboundParameterTransformer
{
	public string? TransformOutbound (object? value)
	{
		return value?.ToString() is null ? null : WordsRegex().Replace(value.ToString()!, "$1-$2").ToLower();
	}

    [GeneratedRegex("([a-z])([A-Z])")]
    private static partial Regex WordsRegex();
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