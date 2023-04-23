using HotChocolate.Utilities;

namespace Cloudey.Reflex.GraphQL.TypeProviders;

public class UlidTypeProvider : IChangeTypeProvider
{
	public bool TryCreateConverter (Type source, Type target, ChangeTypeProvider root, out ChangeType? converter)
	{
		converter = null;

		if (source == typeof(Ulid) && target == typeof(string))
		{
			converter = input => ((Ulid)input!).ToString();
			return true;
		}
		
		if (source == typeof(string) && target == typeof(Ulid))
		{
			converter = input => Ulid.Parse((string)input!);
			return true;
		}
		
		if (source == typeof(Ulid?) && target == typeof(string))
		{
			converter = input => ((Ulid?)input)?.ToString();
			return true;
		}
		
		if (source == typeof(string) && target == typeof(Ulid?))
		{
			converter = input => Ulid.TryParse((string)input!, out var ulid) ? ulid : null;
			return true;
		}
		
		if (source == typeof(Guid) && target == typeof(Ulid))
		{
			converter = input => new Ulid((Guid)input!);
			return true;
		}
		
		if (source == typeof(Ulid) && target == typeof(Guid))
		{
			converter = input => ((Ulid)input!).ToGuid();
			return true;
		}

		return false;
	}
}