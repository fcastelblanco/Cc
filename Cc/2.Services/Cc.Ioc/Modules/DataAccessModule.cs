using Autofac;
using Cc.Upt.Data.Definitions;
using Cc.Upt.Data.Implementations;

namespace Cc.Ioc.Modules
{
    public class DataAccessModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<Context>().As<IContext>();
        }
    }
}