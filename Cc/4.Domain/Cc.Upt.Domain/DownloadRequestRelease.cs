using System;
using System.ComponentModel.DataAnnotations.Schema;

using Cc.Upt.CommonDomain.Implementations;
using Cc.Upt.Domain.Enumerations;

namespace Cc.Upt.Domain
{
    [Table("DownloadRequestRelease", Schema = "dbo")]
    public class DownloadRequestRelease : AuditableEntity<Guid>
    {
        public Guid ReleaseId { get; set; }
        public DownloadRequestReleaseStatusType DownloadRequestReleaseStatusType { get; set; }
    }
}
