using Cloudey.Reflex.Database.ValueGenerators;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cloudey.Reflex.Database;

public abstract class EntityConfiguration<T> : IEntityTypeConfiguration<T> where T : class, IEntity
{
	public virtual void Configure (EntityTypeBuilder<T> builder)
	{
		builder.Property(e => e.Id)
			.ValueGeneratedOnAdd()
			.HasValueGenerator<UlidValueGenerator>()
			.HasValueGeneratorFactory<UlidValueGeneratorFactory>();
		
		builder.Property(e => e.Revision)
			.ValueGeneratedOnAddOrUpdate()
			.HasValueGenerator<UlidValueGenerator>()
			.HasValueGeneratorFactory<UlidValueGeneratorFactory>();;
		
		builder.Property(e => e.Created)
			.ValueGeneratedOnAdd()
			.HasValueGenerator<CurrentDateTimeValueGenerator>()
			.HasValueGeneratorFactory<CurrentDateTimeValueGeneratorFactory>();
		
		builder.Property(e => e.Updated)
			.ValueGeneratedOnUpdate()
			.HasValueGenerator<CurrentDateTimeValueGenerator>()
			.HasValueGeneratorFactory<CurrentDateTimeValueGeneratorFactory>();

		builder.Ignore(e => e.Guid);
	}
}