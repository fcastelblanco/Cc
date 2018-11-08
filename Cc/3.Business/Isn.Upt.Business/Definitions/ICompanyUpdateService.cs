using System;
using System.Collections.Generic;
using Isn.Upt.Data.Definitions;
using Isn.Upt.Domain;

namespace Isn.Upt.Business.Definitions
{
    public interface ICompanyUpdateService : IEntityService<CompanyUpdate>
    {
        List<CompanyUpdate> GetCompanyUpdateList(Guid companyId);
        bool Save(CompanyUpdate companyUpdate);
        CompanyUpdate ValidateXmlFile(string path, string userName);
        CompanyUpdate GetLastUpdate(Guid companyId);
        bool CreateXml(CompanyUpdate companyUpdate, string path);
        IEnumerable<Release> GetAvailableReleaseByCompanyId(Guid companyId);
    }
}