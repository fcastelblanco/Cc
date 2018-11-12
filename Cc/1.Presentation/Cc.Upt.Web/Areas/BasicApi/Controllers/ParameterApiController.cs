using System.Web.Http;
using Cc.Upt.Business.Definitions;
using Cc.Upt.Web.AuthenticationApi;

namespace Cc.Upt.Web.Areas.BasicApi.Controllers
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