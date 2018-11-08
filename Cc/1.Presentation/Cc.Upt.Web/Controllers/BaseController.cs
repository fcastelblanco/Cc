using System.Linq;
using System.Web.Mvc;
using Cc.Upt.Web.AuthenticationWeb;
using Isn.Upt.Business.Definitions;
using Isn.Upt.Business.Implementations.Singleton;

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