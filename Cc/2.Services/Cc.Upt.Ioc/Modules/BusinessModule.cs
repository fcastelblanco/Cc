using Autofac;
using Cc.Upt.Business.Definitions;
using Cc.Upt.Business.Implementations;

namespace Cc.Upt.Ioc.Modules
{
    public class BusinessModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<CompanyReleaseService>().As<ICompanyReleaseService>();
            builder.RegisterType<CompanyService>().As<ICompanyService>();
            builder.RegisterType<CompanyUpdateService>().As<ICompanyUpdateService>();
            builder.RegisterType<DataBaseProviderService>().As<IDataBaseProviderService>();
            builder.RegisterType<DownloadRequestReleaseService>().As<IDownloadRequestReleaseService>();
            
            builder.RegisterType<GenericClientService>().As<IGenericClientService>();
            builder.RegisterType<ParameterService>().As<IParameterService>();
            builder.RegisterType<ReleaseService>().As<IReleaseService>();
            builder.RegisterType<UserService>().As<IUserService>();
            builder.RegisterType<UserTokenService>().As<IUserTokenService>();
            
            builder.RegisterType<ValidateService>().As<IValidateService>();
        }
    }
}