using Autofac;
using EntityFrameworkCore.Triggers;

namespace Cloudey.Reflex.Database;

public class DatabaseModule : Module
{
	protected override void Load (ContainerBuilder builder)
	{
		builder.RegisterGeneric(typeof(Triggers<,>))
			.As(typeof(ITriggers<,>))
			.SingleInstance();

		builder.RegisterGeneric(typeof(Triggers<>))
			.As(typeof(ITriggers<>))
			.SingleInstance();

		builder.RegisterType(typeof(Triggers))
			.As(typeof(ITriggers))
			.SingleInstance();
	}
}