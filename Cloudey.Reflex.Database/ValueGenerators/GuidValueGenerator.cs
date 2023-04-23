using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace Cloudey.Reflex.Database.ValueGenerators;

public class GuidValueGenerator : ValueGenerator<Guid>
{
	public override bool GeneratesTemporaryValues => false;

	public override Guid Next (EntityEntry entry)
	{
		return Ulid.NewUlid().ToGuid();
	}
}