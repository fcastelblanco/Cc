using Isn.Upt.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Isn.Upt.Business.Definitions
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
