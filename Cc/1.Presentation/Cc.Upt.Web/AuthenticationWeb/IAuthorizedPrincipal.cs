using System;
using System.Security.Principal;
using Isn.Upt.Domain.Enumerations;

namespace Cc.Upt.Web.AuthenticationWeb
{
    public interface IAuthorizedPrincipal : IPrincipal
    {
        Guid Id { get; set; }
        string Name { get; set; }
        string LastName { get; set; }
        string UserName { get; set; }
        Profile Profile { get; set; }
        Guid CompanyId { get; set; }
    }
}