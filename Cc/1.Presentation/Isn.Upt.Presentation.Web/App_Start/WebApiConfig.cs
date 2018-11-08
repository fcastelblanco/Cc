using System.Web.Http;
using Isn.Upt.Presentation.AuthenticationApi;
using Isn.Upt.Presentation.ExceptionHandling;
using Newtonsoft.Json;

namespace Isn.Upt.Presentation
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