using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace Cloudey.Reflex.Database.ValueGenerators;

public class CurrentDateTimeValueGenerator : ValueGenerator<DateTime>
{
	public override bool GeneratesTemporaryValues => false;

	public override DateTime Next (EntityEntry entry) => DateTime.UtcNow;
}

public class CurrentDateTimeValueGeneratorFactory : ValueGeneratorFactory
{
	public override ValueGenerator Create (IProperty property, ITypeBase typeBase) =>
		new CurrentDateTimeValueGenerator();
}