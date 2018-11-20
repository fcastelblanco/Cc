using System;
using System.ComponentModel.DataAnnotations.Schema;
using Cc.Upt.Domain.Common.Implementations;

namespace Cc.Upt.Domain
{
    [Table("ServerUpdate", Schema = "dbo")]
    public class ServerUpdate : AuditableEntity
    {
        public DateTime Update { get; set; }
        [ForeignKey("Release")]
        public Guid ReleaseId { get; set; }
        [ForeignKey("Server")]
        public Guid ServerId { get; set; }
        
        public virtual  Release Release { get; set; }
        public virtual  Server Server { get; set; }
    }
}