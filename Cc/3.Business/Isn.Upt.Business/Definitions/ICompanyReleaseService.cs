using System;
using System.Collections.Generic;
using Isn.Upt.Data.Definitions;
using Isn.Upt.Domain;

namespace Isn.Upt.Business.Definitions
{
    public interface ICompanyReleaseService : IEntityService<CompanyRelease>
    {
        bool Save(CompanyRelease model);
        List<CompanyRelease> GetCompanyReleaseList(Guid companyId);
        List<Guid> GetReleaseListDownload(IEnumerable<Guid> releaselist, Guid companyId);
        IEnumerable<CompanyRelease> GetCompanyReleaseListByReleaseId(Guid releaseId);
    }
}