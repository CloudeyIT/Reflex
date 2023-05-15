using Cloudey.Reflex.Core.Setup;
using Cloudey.Reflex.Database.ValueConverters;
using Cloudey.Reflex.Database.ValueGenerators;
using EntityFrameworkCore.Triggers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Cloudey.Reflex.Database;

public abstract class ReflexDatabaseContext<TUser, TRole, TContext> : IdentityDbContext<TUser, TRole, Ulid>
	where TUser : IdentityUser<Ulid> 
	where TRole : IdentityRole<Ulid>
	where TContext : ReflexDatabaseContext<TUser, TRole, TContext>
{
	public ReflexDatabaseContext (DbContextOptions<TContext> options) : base(options) { }

	protected override void ConfigureConventions (ModelConfigurationBuilder configurationBuilder)
	{
		base.ConfigureConventions(configurationBuilder);
		
		configurationBuilder.Properties<Ulid>()
			.HaveConversion<UlidToGuidConverter>();
	}
	
	protected override void OnModelCreating (ModelBuilder builder)
	{
		base.OnModelCreating(builder);

		builder.Entity<TUser>()
			.Property(u => u.Id)
			.ValueGeneratedOnAdd()
			.HasValueGenerator<UlidValueGenerator>()
			.HasValueGeneratorFactory<UlidValueGeneratorFactory>();
		
		builder.Entity<TRole>()
			.Property(u => u.Id)
			.ValueGeneratedOnAdd()
			.HasValueGenerator<UlidValueGenerator>()
			.HasValueGeneratorFactory<UlidValueGeneratorFactory>();
		
		AppDomain.CurrentDomain.GetIncludedAssemblies()
			.ForEach(assembly => builder.ApplyConfigurationsFromAssembly(assembly));
	}

	public override int SaveChanges ()
	{
		return this.SaveChangesWithTriggers(base.SaveChanges);
	}

	public override int SaveChanges (bool acceptAllChangesOnSuccess)
	{
		return this.SaveChangesWithTriggers(base.SaveChanges, acceptAllChangesOnSuccess);
	}

	public override Task<int> SaveChangesAsync (CancellationToken cancellationToken = default)
	{
		return this.SaveChangesWithTriggersAsync(base.SaveChangesAsync, true, cancellationToken);
	}

	public override Task<int> SaveChangesAsync (
		bool acceptAllChangesOnSuccess,
		CancellationToken cancellationToken = default
	)
	{
		return this.SaveChangesWithTriggersAsync(
			base.SaveChangesAsync,
			acceptAllChangesOnSuccess,
			cancellationToken
		);
	}
}