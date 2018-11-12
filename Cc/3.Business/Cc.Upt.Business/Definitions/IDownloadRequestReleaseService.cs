using System;
using Cc.Upt.Domain;

namespace Cc.Upt.Business.Definitions
{
    public interface IDownloadRequestReleaseService
    {
        bool CreateDownloadRequestRelease(DownloadRequestRelease downloadRequestRelease);
        bool UpdateDownloadRequestRelease(DownloadRequestRelease downloadRequestRelease);
        DownloadRequestRelease GetDownloadRequestReleaseByReleaseId(Guid releaseId);
        void ExecuteRequestReleaseCreator();
        bool IncreaseDownloadRequestReleaseDate(DownloadRequestRelease downloadRequestRelease);
    }
}
