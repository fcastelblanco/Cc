using System.Web.Http;
using Isn.Upt.Business.Definitions;
using Isn.Upt.Presentation.AuthenticationApi;

namespace Isn.Upt.Presentation.Areas.BasicApi.Controllers
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