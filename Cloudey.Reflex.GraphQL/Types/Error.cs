namespace Cloudey.Reflex.GraphQL.Types;

public record Error(string Message, string Code) : IError;