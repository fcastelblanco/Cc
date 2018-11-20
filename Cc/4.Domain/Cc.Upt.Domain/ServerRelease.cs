using System;
using System.ComponentModel.DataAnnotations.Schema;
using Cc.Upt.Domain.Common.Implementations;

namespace Cc.Upt.Domain
{
    [Table("ServerRelease", Schema = "dbo")]
    public class ServerRelease : AuditableEntity
    {
        [ForeignKey("Release")]
        public Guid ReleaseId { get; set; }
        [ForeignKey("Server")]
        public Guid ServerId { get; set; }
        
        public virtual  Release Release { get; set; }
        public virtual  Server Server { get; set; }
    }
}