using System.Linq;
using System.Web.Mvc;
using Cc.Upt.Web.Controllers;
using Isn.Upt.Business.Definitions;
using Isn.Upt.Business.Implementations.Singleton;
using Isn.Upt.Domain;
using Isn.Upt.Domain.Enumerations;

namespace Cc.Upt.Web.Areas.Basic.Controllers
{
    [Authorize]
    public class ParameterController : BaseController
    {
        private readonly IParameterService _parameterService;

        public ParameterController(IParameterService parameterService)
        {
            _parameterService = parameterService;
        }

        public ActionResult Index()
        {
            _parameterService.PrepareData();
            var dataRetrieved = _parameterService.GetAllParameters();
            ParameterSingleton.Instance = new ParameterSingleton
            {
                ParameterList = _parameterService.GetAllParameters().ToList()
            };
            return View(dataRetrieved);
        }

        public ActionResult Save(Parameter parameter)
        {
            if (parameter.ParameterType == ParameterType.CheckBox)
            {
                parameter.Value = Request.Form["Value"] == null ? false.ToString() : true.ToString();
            }

            _parameterService.Save(parameter, true);
            return RedirectToAction("Index");
        }
    }
}