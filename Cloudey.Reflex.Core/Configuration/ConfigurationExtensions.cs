using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace Cloudey.Reflex.Core.Configuration;

public static class ConfigurationExtensions
{
	public static WebApplicationBuilder AddReflexConfiguration (this WebApplicationBuilder builder)
	{
		builder.Configuration.AddJsonFile("appsettings.json", true, true);
		builder.Configuration.AddYamlFile("appsettings.yaml", true, true);
		builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true, true);
		builder.Configuration.AddYamlFile($"appsettings.{builder.Environment.EnvironmentName}.yaml", true, true);
		builder.Configuration.AddJsonFile("appsettings.Local.json", true, true);
		builder.Configuration.AddYamlFile("appsettings.Local.yaml", true, true);
		builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.Local.json", true, true);
		builder.Configuration.AddYamlFile($"appsettings.{builder.Environment.EnvironmentName}.Local.yaml", true, true);
		builder.Configuration.AddEnvironmentVariables("APP__");
		
		return builder;
	}
}