using EntityFrameworkCore.Triggers;

namespace Cloudey.Reflex.Database;

public interface IEntity
{
	static IEntity ()
	{
		Triggers<IEntity>.GlobalInserting.Add(entity => entity.Entity.Created = DateTime.UtcNow);
		Triggers<IEntity>.GlobalInserting.Add(entity => entity.Entity.Revision = Ulid.NewUlid());
		Triggers<IEntity>.GlobalUpdating.Add(entity => entity.Entity.Updated = DateTime.UtcNow);
		Triggers<IEntity>.GlobalUpdating.Add(entity => entity.Entity.Revision = Ulid.NewUlid());
	}
	
	public Ulid Id { get; set; }
	public Guid Guid
	{
		get => Id.ToGuid();
		set => Id = new Ulid(value);
	}
	public DateTime Created { get; set; }
	public DateTime? Updated { get; set; }
	public Ulid Revision { get; set; }
}