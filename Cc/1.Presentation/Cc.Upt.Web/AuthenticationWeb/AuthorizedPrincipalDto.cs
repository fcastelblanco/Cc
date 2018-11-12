using System;
using Cc.Upt.Domain.Enumerations;

namespace Cc.Upt.Web.AuthenticationWeb
{
    public class AuthorizedPrincipalDto 
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public Profile Profile { get; set; }
        public Guid CompanyId { get; set; }
         
    }
}