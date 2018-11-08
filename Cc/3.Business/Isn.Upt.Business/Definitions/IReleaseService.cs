using System;
using System.Collections.Generic;
using Isn.Upt.Data.Definitions;
using Isn.Upt.Domain;

namespace Isn.Upt.Business.Definitions
{
    public interface IReleaseService : IEntityService<Release>
    {
        IEnumerable<Release> GetList();
        bool SetReleaseAsSafe(bool isSafe, Guid releaseId);
        IEnumerable<Release> GetLatestRelease(Guid releaseId);
        Release GetReleaseById(Guid releaseId);
    }
}