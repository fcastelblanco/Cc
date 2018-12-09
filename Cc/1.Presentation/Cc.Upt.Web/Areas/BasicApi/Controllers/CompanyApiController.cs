using System;
using System.Web.Http;
using Cc.Upt.Business.Definitions;
using Cc.Upt.Domain;
using Cc.Upt.Web.Base;

namespace Cc.Upt.Web.Areas.BasicApi.Controllers
{
    [Authorize]
    [RoutePrefix("api")]
    public class CompanyApiController : BaseApiController
    {
        private readonly ICompanyService _companyService;
        private readonly ICompanyUpdateService _companyUpdateService;

        public CompanyApiController(ICompanyService companyService, ICompanyUpdateService companyUpdateService)
        {
            _companyService = companyService;
            _companyUpdateService = companyUpdateService;
        }

        [HttpGet]
        [Route("CompanyApi/GetCompanyUpdateList/{companyId}")]
        public IHttpActionResult GetCompanyUpdateList(Guid companyId)
        {
            var dataToReturn = _companyUpdateService.GetServerUpdateList(companyId);
            return Ok(dataToReturn);
        }

        [HttpGet]
        [Route("CompanyApi/GetAvailableReleaseByCompanyId/{companyId}")]
        public IHttpActionResult GetAvailableReleaseByCompanyId(Guid companyId)
        {
            var dataToReturn = _companyUpdateService.GetAvailableReleaseByServerId(companyId);
            return Ok(dataToReturn);
        }

        [HttpGet]
        [Route("CompanyApi/GetCompany/{id}")]
        public IHttpActionResult GetCompany(Guid id)
        {
            var dataToReturn = _companyService.GetById(id);
            return Ok(dataToReturn);
        }

        [HttpGet]
        [Route("CompanyApi/GetCompanyByName/{name}")]
        public IHttpActionResult GetCompanyByName(string name)
        {
            var dataToReturn = _companyService.GetByName(name);
            return Ok(dataToReturn);
        }

        [HttpPost]
        public IHttpActionResult AddCompanyUpdate(ServerUpdate companyUpdate)
        {
            try
            {
                var dataRetireved = _companyUpdateService.Save(companyUpdate);
                return Ok(dataRetireved);
            }
            catch (Exception e)
            {
                ModelState.AddModelError("Error", e.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpGet]
        [Route("CompanyApi/GetCompanyListByPeriodSupportOpen")]
        public IHttpActionResult GetCompanyListByPeriodSupportOpen()
        {
            try
            {
                var dataRetrieved = _companyService.GetCompanyListByPeriodSupportOpen();
                return Ok(dataRetrieved);
            }
            catch (Exception e)
            {

                ModelState.AddModelError("Error", e.Message);
                return BadRequest(ModelState);
            }
        }
    }
}