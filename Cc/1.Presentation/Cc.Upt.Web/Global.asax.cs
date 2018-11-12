using System;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Script.Serialization;
using System.Web.Security;
using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using Cc.Ioc;
using Cc.Upt.Business.Definitions;
using Cc.Upt.Web.AuthenticationWeb;

namespace Cc.Upt.Web
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            var builder = new ContainerBuilder();

            builder.RegisterModules();
            builder.RegisterAssemblyTypes(typeof(MvcApplication).Assembly).PropertiesAutowired();

            var container = builder.Build();

            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            GlobalConfiguration.Configure(WebApiConfig.Register);
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            DependencyResolver.Current.GetService<IParameterService>().PrepareData();
        }

        protected void Application_PostAuthenticateRequest()
        {
            var authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie != null)
            {
                var authTicket = FormsAuthentication.Decrypt(authCookie.Value);

                if (authTicket != null)
                {
                    var serializeModel =
                        new JavaScriptSerializer().Deserialize<AuthorizedPrincipalDto>(authTicket.UserData);
                    var userService = DependencyResolver.Current.GetService<IUserService>();
                    var user = userService.FindBy(u => u.UserName == serializeModel.UserName).FirstOrDefault();
                    if (user != null)
                    {
                        var newUser = new AuthorizedPrincipal(authTicket.UserData)
                        {
                            Id = user.Id,
                            Name = user.Name,
                            LastName = user.LastName,
                            UserName = user.UserName,
                            Profile = user.Profile,
                            CompanyId = user.CompanyId
                        };


                        HttpContext.Current.User = newUser;
                        if (!Request.Path.ToLower().Contains("logout"))
                            return;

                        authCookie.Expires = DateTime.Now.AddDays(-2);
                        Request.Cookies.Remove(FormsAuthentication.FormsCookieName);
                        Request.Cookies.Clear();
                    }
                }
            }
        }
    }
}