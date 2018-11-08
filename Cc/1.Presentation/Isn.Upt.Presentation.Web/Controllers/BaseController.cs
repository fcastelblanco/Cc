using System.Linq;
using System.Web.Mvc;
using Isn.Upt.Business.Definitions;
using Isn.Upt.Business.Implementations.Singleton;
using Isn.Upt.Presentation.AuthenticationWeb;

namespace Isn.Upt.Presentation.Controllers
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