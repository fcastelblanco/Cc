using System.Web.Http;
using Cc.Upt.Web.AuthenticationApi;
using Isn.Upt.Business.Definitions;

namespace Cc.Upt.Web.Areas.BasicApi.Controllers
{
    public class UserApiController : AuthorizedApiController
    {
        private readonly IUserService _userService;

        public UserApiController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public IHttpActionResult GetCurrentUserData()
        {
            return Ok(AuthorizedUser);
        }
    }
}