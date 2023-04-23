using EntityFrameworkCore.Triggers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Cloudey.Reflex.Database;

public static class DatabaseSetupExtensions
{
	public static IServiceCollection AddDatabase<TContext> (this IServiceCollection services, IConfiguration configuration) where TContext : DbContext
	{
		var migrationsAssembly = configuration.GetValue<string>("MigrationsAssembly");

		var mainDbConfig = configuration.GetSection("Database").GetSection(typeof(TContext).Name);

		services.AddPooledDbContextFactory<TContext>(
			builder => builder
				.UseNpgsql(
					new NpgsqlConnectionStringBuilder
					{
						Host = mainDbConfig.GetValue<string>("Host"),
						Port = mainDbConfig.GetValue<int>("Port"),
						Username = mainDbConfig.GetValue<string>("User"),
						Password = mainDbConfig.GetValue<string>("Password"),
						Database = mainDbConfig.GetValue<string>("Database"),
					}.ConnectionString,
					npgsql =>
					{
						npgsql.MigrationsAssembly(migrationsAssembly);
						npgsql.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery);
					}
				)
				.EnableDetailedErrors()
				.EnableSensitiveDataLogging()
		);

		services.AddScoped(provider => provider.GetRequiredService<IDbContextFactory<TContext>>().CreateDbContext());

		return services;
	}
}