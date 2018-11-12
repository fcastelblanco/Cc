﻿using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using Cc.Upt.Business.Definitions;
using Cc.Upt.Common.ExtensionMethods;
using Cc.Upt.Domain.Dto;
using Cc.Upt.Web.AuthenticationWeb;
using Cc.Upt.Web.Controllers;


namespace Cc.Upt.Web.Areas.Basic.Controllers
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