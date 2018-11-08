using System;
using System.Collections.Generic;
using Isn.Upt.Data.Definitions;
using Isn.Upt.Domain;

namespace Isn.Upt.Business.Definitions
{
    public interface ICompanyService : IEntityService<Company>
    {
        Company GetById(Guid id);
        Company GetByName(string name);
        bool Save(Company user);
        IEnumerable<Company> GetAllCompanys();
        IEnumerable<Company> GetCompanyListByPeriodSupportOpen();
    }
}