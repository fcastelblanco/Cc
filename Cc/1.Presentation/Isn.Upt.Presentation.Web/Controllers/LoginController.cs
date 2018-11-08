using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;
using Isn.Common.ExtensionMethods;
using Isn.Upt.Business.Definitions;
using Isn.Upt.Business.Implementations.Singleton;
using Isn.Upt.Domain;
using Isn.Upt.Domain.Dto;
using Isn.Upt.Domain.Enumerations;
using Isn.Upt.Presentation.AuthenticationWeb;


namespace Isn.Upt.Presentation.Controllers
{
    public class LoginController : BaseController
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
            return View();
        }

        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginDto model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Nombre de usuario y clave son requeridos");
                return RedirectToAction("Index");
            }

            var currentUserPassword = model.Password.Encode();
            var currentUser =
                _userService.FindBy(u => u.UserName == model.UserName && u.Password == currentUserPassword)
                    .FirstOrDefault();
            if (currentUser == null)
            {
                ModelState.AddModelError(string.Empty, "Nombre de usuario o clave son incorrectos");
                return View("Index");
            }
            
            var autorizaPrincipal = new AuthorizedPrincipal(model.UserName)
            {
                Id = currentUser.Id,
                Name = currentUser.Name,
                LastName = currentUser.LastName,
                UserName = currentUser.UserName,
                Profile = currentUser.Profile,
                CompanyId = currentUser.CompanyId
            };

            CrearAutenticacionTicket(autorizaPrincipal);
            _userTokenService.DeleteAllUserTokenByTokenType(currentUser.Id, TokenType.CreateUser);

            ParameterSingleton.Instance = new ParameterSingleton
            {
                ParameterList = _parameterService.GetAllParameters().ToList()
            };

            return RedirectToLocal(returnUrl);
        }

        public void CrearAutenticacionTicket(IAuthorizedPrincipal authorizedData)
        {
            var userData = new JavaScriptSerializer().Serialize(new AuthorizedPrincipalDto
            {
                Id = authorizedData.Id,
                Name = authorizedData.Name,
                LastName = authorizedData.LastName,
                UserName = authorizedData.UserName,
                Profile = authorizedData.Profile,
                CompanyId = authorizedData.CompanyId
            });

            var authenticationTicket = new FormsAuthenticationTicket(1, authorizedData.UserName, DateTime.Now,
                DateTime.Now.AddDays(0.5), false, userData);
            var encryptedTicket = FormsAuthentication.Encrypt(authenticationTicket);
            var theCookie = new HttpCookie("_RetadpUnoiculosI", encryptedTicket);
            Response.Cookies.Add(theCookie);
        }

        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            return RedirectToAction("Index");
        }

        public ActionResult RecoverPassword()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        public ActionResult VerifyPasswordRecover(LoginDto model)
        {
            var currentUser = _userService.FindBy(x => x.Email == model.Email).FirstOrDefault();
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

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.Route("Login/CreateUserPassword/{token}/{tokenType}")]
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
                return RedirectToAction("Index");
            }

            throw new Exception("No fue posible lleva a cabo toda la operación");
        }

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.Route("Login/ForgotPassword/{token}/{tokenType}")]
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
                var currentUserPassword = model.Password.Encode();
                validUser.Password = currentUserPassword;
                _userService.Update(validUser);
                return RedirectToAction("PasswordChanged", "Password");
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
    }
}