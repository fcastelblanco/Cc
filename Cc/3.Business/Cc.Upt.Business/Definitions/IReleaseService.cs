using System;
using System.Collections.Generic;
using Cc.Upt.Data.Definitions;
using Cc.Upt.Domain;

namespace Cc.Upt.Business.Definitions
{
    public interface IReleaseService : IEntityService<Release>
    {
        IEnumerable<Release> GetList();
        bool SetReleaseAsSafe(bool isSafe, Guid releaseId);
        IEnumerable<Release> GetLatestRelease(Guid releaseId);
        Release GetReleaseById(Guid releaseId);
    }
}