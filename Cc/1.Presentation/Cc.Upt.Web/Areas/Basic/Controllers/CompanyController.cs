using System;
using System.Web.Mvc;
using Cc.Upt.Business.Definitions;
using Cc.Upt.Domain;
using Cc.Upt.Web.AuthenticationWeb;
using Cc.Upt.Web.Controllers;


namespace Cc.Upt.Web.Areas.Basic.Controllers
{
    [Authorized]
    public class CompanyController : BaseController
    {
        private readonly ICompanyService _companyService;

        public CompanyController(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        public ActionResult Index()
        {
            var dataRetrieved = _companyService.GetAllCompanys();
            return View(dataRetrieved);
        }

        public ActionResult Create()
        {
            return View();
        }

        public ActionResult Edit(Guid id)
        {
            var dataCompany = _companyService.GetById(id);
            return View(dataCompany);
        }

        public ActionResult Save(Company model)
        {
            var currentCompany = _companyService.GetById(model.Id);

            if (!ModelState.IsValid)
            {
                return currentCompany == null ? View("Create") : View("Edit", model); 
            }

            if (model.DateEndSupport.Date < DateTime.Now.Date)
            {
                ModelState.AddModelError(string.Empty, @"La fecha de finalizacion de soporte no puede ser inferior a la actual");
                return currentCompany == null ? View("Create") : View("Edit", model);
            }

            var dataRetrieved = _companyService.Save(model);
            return RedirectToAction("Index");
        }
    }
}
