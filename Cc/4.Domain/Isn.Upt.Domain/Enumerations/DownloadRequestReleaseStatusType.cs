using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Isn.Upt.Domain.Enumerations
{
    public enum DownloadRequestReleaseStatusType
    {
        Creating,
        Ready,
        Using,
        Available,
        RequestedForCreate
    }
}
