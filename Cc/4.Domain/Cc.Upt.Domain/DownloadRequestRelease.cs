using System;
using System.ComponentModel.DataAnnotations.Schema;
using Cc.Upt.Domain.Common.Implementations;
using Cc.Upt.Domain.Enumerations;

namespace Cc.Upt.Domain
{
    [Table("DownloadRequestRelease", Schema = "dbo")]
    public class DownloadRequestRelease : AuditableEntity
    {
        public Guid ReleaseId { get; set; }
        public DownloadRequestReleaseStatusType DownloadRequestReleaseStatusType { get; set; }

        public virtual Release Release { get; set; }
    }
}
