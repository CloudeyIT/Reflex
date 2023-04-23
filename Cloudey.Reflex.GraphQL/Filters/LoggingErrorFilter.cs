using Serilog;

namespace Cloudey.Reflex.GraphQL.Filters;

public class LoggingErrorFilter : IErrorFilter
{
	public IError OnError (IError error)
	{
		if (error.Exception is not null)
		{
			Log.Error(error.Exception, "{Message}: {Exception}", error.Message, error.Exception?.Message);
			return error;
		}

		Log.Warning("{Message}", error.Message);
		return error;
	}
}