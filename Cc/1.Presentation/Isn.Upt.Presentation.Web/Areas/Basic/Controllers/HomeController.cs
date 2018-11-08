using System;
using System.Web.Mvc;
using Isn.Upt.Business.Definitions;
using Isn.Upt.Presentation.AuthenticationWeb;
using Isn.Upt.Presentation.Controllers;
using Isn.Upt.Presentation.ExceptionHandling;
using NLog;

namespace Isn.Upt.Presentation.Areas.Basic.Controllers
{
   
    [Authorized]
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