using Autofac;
using Cc.Upt.Ioc.Modules;

namespace Cc.Upt.Ioc
{
    public static class CcContainer
    {
        private static IContainer _container;

        public static IContainer Container
        {
            get
            {
                if (_container != null) return _container;

                var builder = new ContainerBuilder();

                builder.RegisterModule(new DataAccessModule());
                builder.RegisterModule(new BusinessModule());

                _container = builder.Build();

                return _container;
            }
        }

        public static ContainerBuilder RegisterModules(this ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterModule(new DataAccessModule());
            containerBuilder.RegisterModule(new BusinessModule());

            return containerBuilder;
        }

        public static TService Resolve<TService>()
        {
            return _container.Resolve<TService>();
        }
    }
}