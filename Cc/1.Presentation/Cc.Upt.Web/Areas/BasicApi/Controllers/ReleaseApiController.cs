using System;
using System.Linq;
using System.Web.Http;
using Cc.Upt.Web.AuthenticationApi;
using Cc.Upt.Web.AuthenticationWeb;
using Isn.Upt.Business.Definitions;
using Isn.Upt.Domain;

namespace Cc.Upt.Web.Areas.BasicApi.Controllers
{
    [Authorized]
    [RoutePrefix("api")]
    public class ReleaseApiController : AuthorizedApiController
    {
        private readonly IDownloadRequestReleaseService _downloadRequestReleaseService;
        private readonly IReleaseService _releaseService;
        private readonly ICompanyReleaseService _companyReleaseService;

        public ReleaseApiController(IReleaseService releaseService,
            IDownloadRequestReleaseService downloadRequestReleaseService, ICompanyReleaseService companyReleaseService)
        {
            _releaseService = releaseService;
            _downloadRequestReleaseService = downloadRequestReleaseService;
            _companyReleaseService = companyReleaseService;
        }

        [HttpGet]
        [Route("ReleaseApi/GetList")]
        public IHttpActionResult GetList()
        {
            var dataToReturn = _releaseService.GetList().Select(x => new Release {
                Id = x.Id,
                IsSafe = x.IsSafe,
                Version = x.Version,
                CreatedBy = x.CreatedBy,
                CreatedDate = x.CreatedDate,
                Description = x.Description,
                Notes = x.Notes,
                Published = x.Published,
                UserId = x.UserId

            }).ToList();
            return Ok(dataToReturn);
        }

        [HttpPost]
        public IHttpActionResult CreateDownloadRequestRelease(DownloadRequestRelease downloadRequestRelease)
        {
            var dataToReturn = _downloadRequestReleaseService.CreateDownloadRequestRelease(downloadRequestRelease);
            return Ok(dataToReturn);
        }

        [HttpPost]
        public IHttpActionResult UpdateDownloadRequestRelease(DownloadRequestRelease downloadRequestRelease)
        {
            var dataToReturn = _downloadRequestReleaseService.UpdateDownloadRequestRelease(new DownloadRequestRelease
            {
                Id = downloadRequestRelease.Id,
                DownloadRequestReleaseStatusType = downloadRequestRelease.DownloadRequestReleaseStatusType,
                
            });

            return Ok(dataToReturn);
        }

        [HttpPost]
        public IHttpActionResult IncreaseDownloadRequestReleaseDate(DownloadRequestRelease downloadRequestRelease)
        {
            var dataToReturn = _downloadRequestReleaseService.IncreaseDownloadRequestReleaseDate(new DownloadRequestRelease
            {
                Id = downloadRequestRelease.Id,
                CreatedDate = downloadRequestRelease.CreatedDate
            });

            return Ok(dataToReturn);
        }

        [HttpGet]
        [Route("ReleaseApi/GetDownloadRequestReleaseByReleaseId/{releaseId}")]
        public IHttpActionResult GetDownloadRequestReleaseByReleaseId(Guid releaseId)
        {
            var dataToReturn = _downloadRequestReleaseService.GetDownloadRequestReleaseByReleaseId(releaseId);
            return Ok(dataToReturn);
        }

        [HttpPost]
        public IHttpActionResult SetReleaseAsSafe(Release release)
        {
            try
            {
                var dataRetireved = _releaseService.SetReleaseAsSafe(release.IsSafe, release.Id);
                return Ok(dataRetireved);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", ex.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpGet]
        [Route("ReleaseApi/GetCompanyReleaseListByReleaseId/{releaseId}")]
        public IHttpActionResult GetCompanyReleaseListByReleaseId(Guid releaseId)
        {
            try
            {
                var dataRetireved = _companyReleaseService.GetCompanyReleaseListByReleaseId(releaseId);
                return Ok(dataRetireved);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", ex.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpGet]
        [Route("ReleaseApi/GetReleaseByVersion/{version}")]
        public IHttpActionResult GetReleaseByVersion(string version)
        {
            try
            {
                var dataRetireved = _releaseService.FindBy(x => x.Version == version).FirstOrDefault();
                return Ok(dataRetireved);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", ex.Message);
                return BadRequest(ModelState);
            }
        }
    }
}