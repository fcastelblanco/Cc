using System.Web.Http;
using Cc.Upt.Business.Definitions;
using Cc.Upt.Web.Base;


namespace Cc.Upt.Web.Areas.BasicApi.Controllers
{
    public class UserApiController : BaseApiController
    {
        private readonly IUserService _userService;

        public UserApiController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public IHttpActionResult GetCurrentUserData()
        {
            return Ok(User);
        }
    }
}