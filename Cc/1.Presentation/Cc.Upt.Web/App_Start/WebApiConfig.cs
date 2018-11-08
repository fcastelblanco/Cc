using System.Web.Http;
using Cc.Upt.Web.AuthenticationApi;
using Cc.Upt.Web.ExceptionHandling;
using Newtonsoft.Json;

namespace Cc.Upt.Web
{
    public class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                "DefaultApi",
                "api/{controller}/{action}/{id}",
                new {id = RouteParameter.Optional}
            );

            config.Filters.Add(new BasicAuthenticationAttribute());
            config.Filters.Add(new IsolucionExceptionFilterAttribute());

            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.PreserveReferencesHandling =
                PreserveReferencesHandling.All;
        }
    }
}