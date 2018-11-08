using System.Web.Http;
using Isn.Upt.Business.Definitions;
using Isn.Upt.Presentation.AuthenticationApi;

namespace Isn.Upt.Presentation.Areas.BasicApi.Controllers
{
    public class ParameterApiController : AuthorizedApiController
    {
        private readonly IParameterService _parameterService;

        public ParameterApiController(IParameterService parameterService)
        {
            _parameterService = parameterService;
        }

        [HttpGet]
        public IHttpActionResult GetAllParameterList()
        {
            var dataToReturn = _parameterService.GetAllParameters();
            return Ok(dataToReturn);
        }
    }
}