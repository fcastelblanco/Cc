using System;
using System.Collections.Generic;
using Cc.Upt.Data.Definitions;
using Cc.Upt.Domain;
using Cc.Upt.Domain.Repository.Definitions;

namespace Cc.Upt.Business.Definitions
{
    public interface ICompanyReleaseService : IRepository<ServerRelease>
    {
        bool Save(ServerRelease model);
        List<ServerRelease> GetCompanyReleaseList(Guid companyId);
        List<Guid> GetReleaseListDownload(IEnumerable<Guid> releaselist, Guid companyId);
        IEnumerable<ServerRelease> GetCompanyReleaseListByReleaseId(Guid releaseId);
    }
}