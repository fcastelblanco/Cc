using System.Web;
using System.Web.Http;
using Isn.Upt.Domain;

namespace Cc.Upt.Web.AuthenticationApi
{
    public class AuthorizedApiController : ApiController
    {
        /// <summary>
        ///     User authorized in the API
        /// </summary>
        public User AuthorizedUser => ((ApiIdentity) HttpContext.Current.User.Identity).User;
    }
}