using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Isn.Upt.Domain.Enumerations;

namespace Isn.Upt.Presentation.AuthenticationWeb
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