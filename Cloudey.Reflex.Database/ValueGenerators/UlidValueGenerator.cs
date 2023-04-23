using Microsoft.EntityFrameworkCore.ChangeTracking;
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