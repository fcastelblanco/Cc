using System.Web.Mvc;
using Cc.Upt.Business.Definitions;

using Cc.Upt.Web.Base;


namespace Cc.Upt.Web.Areas.Basic.Controllers
{
   
    [Authorize]
    public class HomeController : BaseController
    {
        private readonly IUserService _userService;

        public HomeController(IUserService userService)
        {
            _userService = userService;
        }

     
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}