using Isn.Common.Implementations;
using Isn.Upt.Domain.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Isn.Upt.Domain
{
    [Table("DownloadRequestRelease", Schema = "dbo")]
    public class DownloadRequestRelease : AuditableEntity<Guid>
    {
        public Guid ReleaseId { get; set; }
        public DownloadRequestReleaseStatusType DownloadRequestReleaseStatusType { get; set; }
    }
}
