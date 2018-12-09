using System;
using System.Web.Mvc;
using Cc.Upt.Business.Definitions;
using Cc.Upt.Domain;
using Cc.Upt.Web.Base;

namespace Cc.Upt.Web.Areas.Basic.Controllers
{
    [Authorize]
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

            var dataRetrieved = _companyService.Save(model);
            return RedirectToAction("Index");
        }
    }
}
