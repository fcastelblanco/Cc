using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using Isn.Common.ExtensionMethods;
using Isn.Upt.Business.Definitions;
using Isn.Upt.Domain.Dto;
using Isn.Upt.Presentation.AuthenticationWeb;
using Isn.Upt.Presentation.Controllers;

namespace Isn.Upt.Presentation.Areas.Basic.Controllers
{
    [Authorized]
    public class PasswordController : BaseController
    {
        private readonly IUserService _userService;

        public PasswordController(IUserService userService)
        {
            _userService = userService;
        }

        public ActionResult ResetPassword(Guid userId)
        {
            var user = _userService.GetById(userId);
            var login = new LoginDto
            {
                UserName = user.UserName,
                Password = user.Password,
                Email = user.Email
            };
            return View(login);
        }

        [ValidateAntiForgeryToken]
        public ActionResult ResetPasswordAction(LoginDto model)
        {
            var validUser = _userService.GetAllUsers().FirstOrDefault(x => x.UserName == model.UserName);
            if (validUser != null && model.Password != null)
            {
                var currentUserPassword = model.Password.Encode();
                validUser.Password = currentUserPassword;
                _userService.Update(validUser);
                FormsAuthentication.SignOut();
                Session.Abandon();
                return RedirectToAction("PasswordChanged");
            }
            else
            {
                //Enviar al log informacion de usuario errado
                FormsAuthentication.SignOut();
                Session.Abandon();
                return RedirectToAction("Index", "Login", new { area = string.Empty });
            }
        }

        public ActionResult PasswordChanged()
        {
            return View();
        }
    }
}