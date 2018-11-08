using System;
using System.Linq;
using System.Web.Mvc;
using Isn.Common.Enumerations;
using Isn.Upt.Business.Definitions;
using Isn.Upt.Domain;
using Isn.Upt.Domain.Enumerations;
using Isn.Upt.Presentation.AuthenticationWeb;
using Isn.Upt.Presentation.Controllers;

namespace Isn.Upt.Presentation.Areas.Basic.Controllers
{
    [Authorized]
    [RoutePrefix("api")]
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
                //if (currentCompany == null)
                //    LoadList(null);
                //else
                //{
                //    LoadList(currentCompany.UpdateMode);
                //}
                
                ModelState.AddModelError(string.Empty, @"La fecha de finalizacion de soporte no puede ser inferior a la actual");
                return currentCompany == null ? View("Create") : View("Edit", model);
            }

            var dataRetrieved = _companyService.Save(model);
            return RedirectToAction("Index");
        }
    }
}
