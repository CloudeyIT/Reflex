using Autofac;
using Autofac.Extensions.DependencyInjection;
using Cloudey.Reflex.Core.Configuration;
using Cloudey.Reflex.Core.Routing;
using Cloudey.Reflex.Core.Setup;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Opw.HttpExceptions.AspNetCore;

namespace Cloudey.Reflex.Core;

public static class SetupExtensions
{
	public static WebApplicationBuilder AddReflexCore (
		this WebApplicationBuilder builder,
		Action<HostBuilderContext, ContainerBuilder>? configureContainer
	)
	{
		builder
			.AddAutofac(configureContainer ?? ((_, _) => { }))
			.AddReflexConfiguration()
			.AddReflexLogging()
			.AddReflexHttp()
			.AddReflexValidation()
			.AddReflexCache();

		return builder;
	}

	public static WebApplicationBuilder AddReflexHttp (this WebApplicationBuilder builder)
	{
		builder.Services.AddCors();
		builder.Services.AddHttpContextAccessor();
		builder.Services.AddControllers();
		builder.Services
			.AddMvc()
			.AddControllersAsServices()
			.UseSlugRoutes()
			.AddHttpExceptions();

		return builder;
	}

	public static WebApplicationBuilder AddReflexValidation (this WebApplicationBuilder builder)
	{
		builder.Services.AddFluentValidationAutoValidation()
			.AddFluentValidationClientsideAdapters();

		builder.Services.AddValidatorsFromAssemblies(AppDomain.CurrentDomain.GetIncludedAssemblies());

		return builder;
	}

	public static WebApplicationBuilder AddReflexCache (this WebApplicationBuilder builder)
	{
		builder.Services.AddMemoryCache();

		return builder;
	}

	public static WebApplicationBuilder AddAutofac (
		this WebApplicationBuilder builder,
		Action<HostBuilderContext, ContainerBuilder> configureContainer
	)
	{
		builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

		builder.Host.ConfigureContainer(configureContainer);

		return builder;
	}
}