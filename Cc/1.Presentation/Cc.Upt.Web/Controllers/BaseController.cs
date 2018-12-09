using System;
using System.Linq;
using System.Security.Claims;
using System.Web.Mvc;
using Cc.Upt.Business.Definitions;
using Cc.Upt.Business.Implementations.Singleton;
using Cc.Upt.Domain;

namespace Cc.Upt.Web.Controllers
{
    public class BaseController : Controller
    {
        protected new User User
        {
            get
            {
                var currentClaimsIdentity = (ClaimsIdentity)System.Web.HttpContext.Current.User.Identity;
                var dataCustomPrincipal = currentClaimsIdentity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

                if (dataCustomPrincipal == null)
                    return null;

                var userService = DependencyResolver.Current.GetService<IUserService>();
                return userService.GetById(Guid.Parse(dataCustomPrincipal.Value));
            }
        }

        public BaseController()
        {
            ParameterSingleton.Instance = new ParameterSingleton
            {
                ParameterList = DependencyResolver.Current.GetService<IParameterService>().GetAllParameters().ToList()
            };
        }
    }
}