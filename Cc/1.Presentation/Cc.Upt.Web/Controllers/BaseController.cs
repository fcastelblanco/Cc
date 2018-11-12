using System.Linq;
using System.Web.Mvc;
using Cc.Upt.Business.Definitions;
using Cc.Upt.Business.Implementations.Singleton;
using Cc.Upt.Web.AuthenticationWeb;


namespace Cc.Upt.Web.Controllers
{
    public class BaseController : Controller
    {
        protected new virtual AuthorizedPrincipal User => HttpContext.User as AuthorizedPrincipal;

        public BaseController()
        {
            ParameterSingleton.Instance = new ParameterSingleton
            {
                ParameterList = DependencyResolver.Current.GetService<IParameterService>().GetAllParameters().ToList()
            };
        }
    }
}