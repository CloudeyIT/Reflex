namespace Cloudey.Reflex.Database;

public interface IEntity
{
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