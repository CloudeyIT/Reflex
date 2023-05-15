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

		builder.Ignore(e => e.Guid);
	}
}