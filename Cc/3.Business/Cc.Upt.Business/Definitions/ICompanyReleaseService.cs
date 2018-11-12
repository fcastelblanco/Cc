using System;
using System.Collections.Generic;
using Cc.Upt.Data.Definitions;
using Cc.Upt.Domain;

namespace Cc.Upt.Business.Definitions
{
    public interface ICompanyReleaseService : IEntityService<CompanyRelease>
    {
        bool Save(CompanyRelease model);
        List<CompanyRelease> GetCompanyReleaseList(Guid companyId);
        List<Guid> GetReleaseListDownload(IEnumerable<Guid> releaselist, Guid companyId);
        IEnumerable<CompanyRelease> GetCompanyReleaseListByReleaseId(Guid releaseId);
    }
}