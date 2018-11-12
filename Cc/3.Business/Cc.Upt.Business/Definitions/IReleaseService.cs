using System;
using System.Collections.Generic;
using Cc.Upt.Data.Definitions;
using Cc.Upt.Domain;
using Cc.Upt.DomainRepository.Definitions;

namespace Cc.Upt.Business.Definitions
{
    public interface IReleaseService : IRepository<Release>
    {
        IEnumerable<Release> GetList();
        bool SetReleaseAsSafe(bool isSafe, Guid releaseId);
        IEnumerable<Release> GetLatestRelease(Guid releaseId);
        Release GetReleaseById(Guid releaseId);
    }
}