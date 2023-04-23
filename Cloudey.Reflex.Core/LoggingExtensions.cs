using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Sentry.Extensibility;
using Sentry.Serilog;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;

namespace Cloudey.Reflex.Core;

public static class LoggingExtensions
{
	public static WebApplicationBuilder AddReflexLogging (this WebApplicationBuilder builder)
	{
		var logLevelConfigured = Enum.TryParse<LogEventLevel>(
			builder.Configuration.GetSection("Logging").GetValue<string?>("Level"),
			out var logLevel
		);

		if (!logLevelConfigured)
		{
			logLevel = builder.Environment.IsProduction() ? LogEventLevel.Information : LogEventLevel.Debug;
			if (builder.Environment.EnvironmentName == "Cli") logLevel = LogEventLevel.Warning;
		}

		var sentryConfig = builder.Configuration.GetSection("Sentry").Get<SentrySerilogOptions>();

		builder.Host.UseSerilog(
			(_, configuration) => configuration
				.Enrich.FromLogContext()
				.Enrich.WithExceptionDetails()
				.Enrich.WithEnvironmentName()
				.Enrich.WithMachineName()
				.WriteTo.Console(logLevel)
				.WriteTo.Debug(logLevel)
				.WriteTo.Sentry(
					sentry =>
					{
						if (sentryConfig?.Dsn is null) return;

						sentry.Dsn = sentryConfig.Dsn;
						sentry.TracesSampleRate = sentryConfig.TracesSampleRate;
						sentry.AttachStacktrace = true;
						sentry.InitializeSdk = true;
						sentry.AutoSessionTracking = true;
					}
				)
		);

		if (sentryConfig?.Dsn is not null)
		{
			builder.WebHost.UseSentry(
				(_, sentry) =>
				{
					sentry.InitializeSdk = false;
					sentry.MaxRequestBodySize = RequestSize.Always;
				}
			);
		}

		return builder;
	}
}