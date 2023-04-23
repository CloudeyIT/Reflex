using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace Cloudey.Reflex.Database.ValueGenerators;

public class CurrentDateTimeValueGenerator : ValueGenerator<DateTime>
{
	public override DateTime Next (EntityEntry entry)
	{
		return DateTime.UtcNow;
	}

	public override bool GeneratesTemporaryValues => false;
}