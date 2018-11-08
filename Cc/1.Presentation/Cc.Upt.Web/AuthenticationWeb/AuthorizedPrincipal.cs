using System;
using System.Security.Principal;
using Isn.Upt.Domain.Enumerations;

namespace Cc.Upt.Web.AuthenticationWeb
{
    public class AuthorizedPrincipal : IAuthorizedPrincipal
    {
        public bool IsInRole(string role)
        {
            throw new System.NotImplementedException();
        }
        public IIdentity Identity { get; }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public Profile Profile { get; set; }
        public Guid CompanyId { get; set; }

        public AuthorizedPrincipal(string userName)
        {
            Identity = new GenericIdentity(userName);
        }
      
    }
}