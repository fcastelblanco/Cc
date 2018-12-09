using System;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using Cc.Upt.Business.Definitions;
using Cc.Upt.Business.Implementations.Singleton;
using Cc.Upt.Common.ExtensionMethods;
using Cc.Upt.Domain;
using Cc.Upt.Domain.DataTransferObject;
using Cc.Upt.Domain.Enumerations;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace Cc.Upt.Web.Controllers
{
    public class LoginController : Controller
    {
        private readonly ICompanyService _companyService;
        private readonly IUserService _userService;
        private readonly IUserTokenService _userTokenService;
        private readonly IParameterService _parameterService;

        public LoginController(IUserService userService, ICompanyService companyService, IUserTokenService userTokenService, IParameterService parameterService)
        {
            _userService = userService;
            _companyService = companyService;
            _userTokenService = userTokenService;
            _parameterService = parameterService;
        }

        public ActionResult Index()
        {
            return View(new LoginDto());
        }

        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginDto model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Nombre de usuario y clave son requeridos");
                return RedirectToAction("Index", model);
            }

            var currentUser =
                _userService.FindBy(u => u.Email == model.Email)
                    .FirstOrDefault();

            if (currentUser == null)
            {
                ModelState.AddModelError(string.Empty, "No se encontró usuario con el e-mail ingresado");
                return View("Index", model);
            }

            if (currentUser.Password.Decrypt(StringExtension.PassPhrase) != model.Password)
            {
                model.Password = string.Empty;
                ModelState.AddModelError(string.Empty, "La clave es incorrecta");
                return View("Index", model);
            }

            var authentication = HttpContext.GetOwinContext().Authentication;

            var identity = new ClaimsIdentity(
                new[]
                {
                    new Claim(ClaimTypes.Name, currentUser.Name),
                    new Claim(ClaimTypes.NameIdentifier, currentUser.Id.ToString())
                }, DefaultAuthenticationTypes.ApplicationCookie);

            authentication.SignIn(new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTime.UtcNow.AddDays(1)
            }, identity);

            _userTokenService.DeleteAllUserTokenByTokenType(currentUser.Id, TokenType.CreateUser);

            ParameterSingleton.Instance = new ParameterSingleton
            {
                ParameterList = _parameterService.GetAllParameters().ToList()
            };

            return RedirectToLocal(returnUrl);
        }

        public ActionResult LogOut()
        {
            var authentication = HttpContext.GetOwinContext().Authentication;
            authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Index", new LoginDto());
        }

        public ActionResult RecoverPassword()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        public ActionResult VerifyPasswordRecover(LoginDto model)
        {
            var currentUser = _userService.FindBy(x => x.Email == model.Email).FirstOrDefault();

            if (currentUser == null)
            {
                ModelState.AddModelError(string.Empty, $"No existe usuario con el correo ingresado");
                return View("RecoverPassword");
            }

            var templatePath =
                System.Web.HttpContext.Current.Server.MapPath("~/Templates/passwordRecovery.html");
            var theUserToken = new UserToken
            {
                Token = Guid.NewGuid(),
                UserId = currentUser.Id,
                TokenType = TokenType.ForgotPassword,
                Expiration = DateTime.Now.Date.AddHours(24)
            };

            return RedirectToAction(_userTokenService.Save(theUserToken)
                ? _userService.RecoverPassword(model.Email,
                $"{Request.Url.Scheme}://{Request.Url.Authority}{Request.ApplicationPath}/Login/ForgotPassword?token={theUserToken.Token}&tokenType={(int)theUserToken.TokenType}",
                templatePath)
                ? "EmailSent"
                : "Index"
                : "Index");
        }

        public ActionResult EmailSent()
        {
            return View();
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Release", new { area = "Basic" });
        }

        [HttpGet]
        [Route("Login/CreateUserPassword/{token}/{tokenType}")]
        public ActionResult CreateUserPassword(string token, int tokenType)
        {
            var dataRetrievedExistingToken = _userTokenService.IsValidToken(Guid.Parse(token), (TokenType)tokenType);

            if (!dataRetrievedExistingToken)
            {
                throw new Exception("The supplied token not exists or is not available");
            }

            var currentUserToken = _userTokenService.GetUserTokenByTokenAndTokenType(Guid.Parse(token), (TokenType)tokenType);

            if (currentUserToken == null)
            {
                throw new Exception("The supplied token not exists");
            }

            return View(new SavePasswordDto
            {
                UserId = currentUserToken.UserId
            });
        }

        public ActionResult SaveUserPassword(SavePasswordDto savePasswordDto)
        {
            if (ModelState.IsValid)
            {
                if (savePasswordDto.Password != savePasswordDto.ConfirmationPassword)
                {
                    ModelState.AddModelError(string.Empty, "La contraseña y su confirmación no son las mismas");
                    return View("CreateUserPassword", savePasswordDto);
                }
            }

            if (!_userService.SavePassword(savePasswordDto.UserId, savePasswordDto.Password))
                throw new Exception("No fue posible lleva a cabo toda la operación");

            if (_userTokenService.DeleteAllUserTokenByTokenType(savePasswordDto.UserId, TokenType.CreateUser))
            {
                return RedirectToAction("Index", new LoginDto());
            }

            throw new Exception("No fue posible lleva a cabo toda la operación");
        }

        [HttpGet]
        [Route("Login/ForgotPassword/{token}/{tokenType}")]
        public ActionResult ForgotPassword(string token, int tokenType)
        {
            var dataRetrievedExistingToken = _userTokenService.IsValidToken(Guid.Parse(token), (TokenType)tokenType);

            if (!dataRetrievedExistingToken)
            {
                throw new Exception("The supplied token not exists or is not available");
            }

            var currentUserToken = _userTokenService.GetUserTokenByTokenAndTokenType(Guid.Parse(token), (TokenType)tokenType);

            if (currentUserToken == null)
            {
                throw new Exception("The supplied token not exists");
            }
            ViewBag.User = currentUserToken.UserId;
            return View();
        }

        [ValidateAntiForgeryToken]
        public ActionResult ResetForgotPassword(LoginDto model, Guid userId)
        {
            var validUser = _userService.GetAllUsers().FirstOrDefault(x => x.Id == userId);
            if (validUser != null)
            {
                var currentUserPassword = model.Password.Encrypt(StringExtension.PassPhrase);
                validUser.Password = currentUserPassword;
                _userService.Update(validUser);
                return RedirectToAction("PasswordChanged", "Password");
            }
            else
            {
                return View("Index", "Login", new LoginDto());
            }
        }
    }
}