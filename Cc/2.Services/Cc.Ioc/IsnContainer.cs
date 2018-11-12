using System.Runtime.Remoting.Contexts;
using Autofac;
using Cc.Ioc.Modules;
using Cc.Upt.Data.Definitions;

namespace Cc.Ioc
{
    public static class IsnContainer
    {
        public static IContainer BaseContainer { get; private set; }

        public static void Build()
        {
            if (BaseContainer != null) return;

            var builder = new ContainerBuilder();
            builder.RegisterType(typeof(Context)).As(typeof(IContext)).InstancePerLifetimeScope();
            builder.RegisterModule(new BusinessModule());
            //builder.RegisterType<GenericClientService>().As<IGenericClientService>();
            BaseContainer = builder.Build();
        }

        public static TService Resolve<TService>()
        {
            return BaseContainer.Resolve<TService>();
        }
    }
}