namespace Cloudey.Reflex.GraphQL.Types;

public interface IError
{
	public string Message { get; init; }
	public string Code { get; init; }
}