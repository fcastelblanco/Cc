using System;
using System.Security.Principal;
using Isn.Upt.Domain;

namespace Isn.Upt.Presentation.AuthenticationApi
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
        public string Name => User.UserName;
        public string AuthenticationType => "Basic";
        public bool IsAuthenticated => true;
    }
}