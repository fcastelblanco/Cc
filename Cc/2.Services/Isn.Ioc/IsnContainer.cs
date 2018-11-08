
using Autofac;
using Isn.Ioc.Modules;
using Isn.Upt.Business.Definitions;
using Isn.Upt.Business.Implementations;
using Isn.Upt.Data.Definitions;
using Isn.Upt.Data.Implementations;

namespace Isn.Ioc
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