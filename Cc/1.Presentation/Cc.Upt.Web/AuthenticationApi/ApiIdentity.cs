using System;
using System.Security.Principal;
using Cc.Upt.Domain;

namespace Cc.Upt.Web.AuthenticationApi
{
    public class ApiIdentity : IIdentity
    {
        public ApiIdentity(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            User = user;
        }

        public User User { get; }
        public string Name => User.Name;
        public string AuthenticationType => "Basic";
        public bool IsAuthenticated => true;
    }
}