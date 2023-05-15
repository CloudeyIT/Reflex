using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace Cloudey.Reflex.Database.ValueGenerators;

public class UlidValueGenerator : ValueGenerator<Ulid>
{
	public override bool GeneratesTemporaryValues => false;

	public override Ulid Next (EntityEntry entry)
	{
		return Ulid.NewUlid();
	}
}

public class UlidValueGeneratorFactory : ValueGeneratorFactory
{
	public override ValueGenerator Create (IProperty property, IEntityType entityType)
	{
		return new UlidValueGenerator();
	}
}