using System;
using System.Linq;
using System.Web.Mvc;
using Cc.Upt.Business.Definitions;
using Cc.Upt.Business.Implementations.Singleton;
using Cc.Upt.Common.Enumerations;
using Cc.Upt.Domain;
using Cc.Upt.Domain.Enumerations;
using Cc.Upt.Web.AuthenticationWeb;
using Cc.Upt.Web.Controllers;

namespace Cc.Upt.Web.Areas.Basic.Controllers
{
    [Authorized] 
    public class UserController : BaseController
    {
        private readonly ICompanyService _companyService;
        private readonly IUserService _userService;
        private readonly IUserTokenService _userTokenService;

        public UserController(IUserService userService, ICompanyService companyService,
            IUserTokenService userTokenService)
        {
            _userService = userService;
            _companyService = companyService;
            _userTokenService = userTokenService;
        }

        public ActionResult Index()
        {
            var dataRetrieved = _userService.GetAllUsers();
            return View(dataRetrieved);
        }

        private void LoadList(Profile? profile)
        {
            ViewBag.ProfileList = Enum.GetValues(typeof(Profile)).Cast<Profile>().Select(x => new SelectListItem
            {
                Value = ((int) x).ToString(),
                Text = x.GetDescription(),
                Selected = profile.HasValue && (int) x == (int) profile
            });
        }

        public ActionResult Create()
        {
            var companiesList = _companyService.GetAllCompanys();
            var showList = new SelectList(companiesList, "Id", "Name");
            ViewBag.Companies = showList;
            LoadList(null);
            return View(new User());
        }

        public ActionResult Edit(Guid id)
        {
            var companiesList = _companyService.GetAllCompanys().ToList();
            var showList = new SelectList(companiesList, "Id", "Name");
            ViewBag.Companies = showList;
            var dataUser = _userService.GetById(id);
            LoadList(dataUser.Profile);
            return View(dataUser);
        }

        public ActionResult Save(User model)
        {
            var theUserToSave = new User
            {
                Profile = model.Profile,
                LastName = model.LastName,
                Name = model.Name,
                UserName = model.UserName,
                Email = model.Email,
                CompanyId = model.CompanyId
            };

            theUserToSave.Id = model.Id == Guid.Empty ? Guid.NewGuid() : model.Id;
            var dataRetrieved = _userService.Save(theUserToSave);

            if (model.Id != Guid.Empty)
                return !dataRetrieved
                    ? RedirectToAction("Edit", "User", new {model.Id})
                    : RedirectToAction("Index", "User");

            var parameterInstance = ParameterSingleton.Instance;

            var theUserToken = new UserToken
            {
                Token = Guid.NewGuid(),
                UserId = theUserToSave.Id,
                TokenType = TokenType.CreateUser,
                Expiration = DateTime.Now.Date.AddDays(Convert.ToInt32(parameterInstance.ParameterList.FirstOrDefault(x => x.ParameterInternalIdentificator == ParameterInternalIdentificator.ValidDaysToken).Value))
            };

            if (!_userTokenService.Save(theUserToken))
                return !dataRetrieved
                    ? RedirectToAction("Edit", "User", new { theUserToSave.Id})
                    : RedirectToAction("Index", "User");

            var templatePath =
                System.Web.HttpContext.Current.Server.MapPath("~/Templates/passwordGenerator.html");
            _userService.CreatePassword(theUserToSave.Password,
                $"{Request.Url.Scheme}://{Request.Url.Authority}{Request.ApplicationPath}/Login/CreateUserPassword?token={theUserToken.Token}&tokenType={(int)theUserToken.TokenType}",
                templatePath, theUserToSave.Email);

            return !dataRetrieved ? RedirectToAction("Edit", "User", new {model.Id}) : RedirectToAction("Index", "User");
        }
    }
}