namespace Cloudey.Reflex.GraphQL.Exceptions;

public class BaseException : Exception
{
	public BaseException (string message, string code) : base(message)
	{
		Code = code;
	}

	public string Code { get; set; }
}