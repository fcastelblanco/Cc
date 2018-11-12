﻿using System;
using System.Collections.Generic;
using Cc.Upt.Data.Definitions;
using Cc.Upt.Domain;

namespace Cc.Upt.Business.Definitions
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