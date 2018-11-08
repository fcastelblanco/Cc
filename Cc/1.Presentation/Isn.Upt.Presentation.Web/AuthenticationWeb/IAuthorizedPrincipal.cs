using System;
using System.Security.Principal;
using Isn.Upt.Domain.Enumerations;

namespace Isn.Upt.Presentation.AuthenticationWeb
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