﻿using System.Reflection;
using Cloudey.Reflex.Core.Setup;
using Cloudey.Reflex.GraphQL.Filters;
using Cloudey.Reflex.GraphQL.Middleware;
using Cloudey.Reflex.GraphQL.TypeProviders;
using HotChocolate.AspNetCore.Serialization;
using HotChocolate.Execution.Configuration;
using HotChocolate.Types.Pagination;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MoreLinq.Extensions;
using IError = Cloudey.Reflex.GraphQL.Types.IError;

namespace Cloudey.Reflex.GraphQL;

public static class HotChocolateBuilderExtensions
{
	public static IRequestExecutorBuilder AddReflexGraphQL (this IRequestExecutorBuilder builder, params Assembly[]? assemblies)
	{
		builder
			.AddTypeExtensionsFromAssemblies(
				assemblies is not null ? assemblies : AppDomain.CurrentDomain.GetIncludedAssemblies()
			)
			.UseExceptions()
			.AddFairyBread(o => o.ThrowIfNoValidatorsFound = false)
			.AddProjections()
			.AddFiltering()
			.AddSorting()
            .ModifyPagingOptions(
                o =>
                {
                    o.IncludeTotalCount = true;
                    o.MaxPageSize = 100;
                    o.DefaultPageSize = 20;
                })
			.AddErrorFilter<LoggingErrorFilter>()
			.UseAutomaticPersistedOperationPipeline()
			.AddInMemoryQueryStorage()
			.AddDefaultTransactionScopeHandler()
			.AddMutationConventions(
				new MutationConventionOptions
				{
					ApplyToAllMutations = true,
					PayloadTypeNamePattern = "{MutationName}Result",
					PayloadErrorTypeNamePattern = "{MutationName}Error",
					PayloadErrorsFieldName = "errors",
					InputArgumentName = "input",
					InputTypeNamePattern = "{MutationName}Input",

				}
			)
			.AddErrorInterfaceType<IError>()
			.AddTypeConverter<UlidTypeProvider>()
			.BindRuntimeType<Ulid, StringType>()
            .InitializeOnStartup();

		builder.Services.AddSingleton(builder);
		builder.Services.RemoveAll<IHttpResponseFormatter>();
		builder.Services.AddSingleton<IHttpResponseFormatter>(new CustomHttpResultFormatter());

		builder.Services.AddSha256DocumentHashProvider();

		return builder;
	}
	
	public static IRequestExecutorBuilder AddTypeExtensionsFromAssemblies (
		this IRequestExecutorBuilder builder,
		IEnumerable<Assembly> assemblies
	)
	{
		assemblies.ForEach(
			assembly =>
			{
				assembly.GetTypes()
					.Where(
						type => type.GetCustomAttribute<QueryTypeAttribute>() is not null ||
						        type.GetCustomAttribute<MutationTypeAttribute>() is not null ||
						        type.GetCustomAttribute<SubscriptionTypeAttribute>() is not null
					)
					.ForEach(type => builder.AddTypeExtension(type));
			}
		);


		return builder;
	}
}