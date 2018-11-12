using System;
using System.Collections.Generic;
using Cc.Upt.Data.Definitions;
using Cc.Upt.Domain;
using Cc.Upt.DomainRepository.Definitions;

namespace Cc.Upt.Business.Definitions
{
    public interface ICompanyUpdateService : IRepository<CompanyUpdate>
    {
        List<CompanyUpdate> GetCompanyUpdateList(Guid companyId);
        bool Save(CompanyUpdate companyUpdate);
        CompanyUpdate ValidateXmlFile(string path, string userName);
        CompanyUpdate GetLastUpdate(Guid companyId);
        bool CreateXml(CompanyUpdate companyUpdate, string path);
        IEnumerable<Release> GetAvailableReleaseByCompanyId(Guid companyId);
    }
}