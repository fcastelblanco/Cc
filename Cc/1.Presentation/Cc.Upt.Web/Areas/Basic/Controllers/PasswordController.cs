using System;
using System.Linq;
using System.Web.Mvc;
using Cc.Upt.Business.Definitions;
using Cc.Upt.Common.ExtensionMethods;
using Cc.Upt.Domain.DataTransferObject;


using Cc.Upt.Web.Base;


namespace Cc.Upt.Web.Areas.Basic.Controllers
{
    [Authorize]
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
                Password = user.Password,
                Email = user.Email
            };
            return View(login);
        }

        [ValidateAntiForgeryToken]
        public ActionResult ResetPasswordAction(LoginDto model)
        {
            var validUser = _userService.GetAllUsers().FirstOrDefault(x => x.Email == model.Email);
            
            if (validUser != null && model.Password != null)
            {
                var currentUserPassword = model.Password.Encrypt(StringExtension.PassPhrase);
                validUser.Password = currentUserPassword;
                _userService.Update(validUser);
                return RedirectToAction("PasswordChanged");
            }
            
            ModelState.AddModelError("", $"El usuario {model.Email} no existe");

            var login = new LoginDto
            {
                Password = model.Password,
                Email = model.Email
            };

            return View("ResetPassword", login);
        }

        public ActionResult PasswordChanged()
        {
            return View();
        }
    }
}