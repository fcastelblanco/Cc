using System;
using System.ComponentModel.DataAnnotations.Schema;
using Cc.Upt.Domain.Common.Implementations;

namespace Cc.Upt.Domain
{
    [Table("Release", Schema = "dbo")]
    public class Release : AuditableEntity
    {
        public string Description { get; set; }
        public string Version { get; set; }
        [ForeignKey("User")]
        public Guid UserId { get; set; }
        public DateTime Published { get; set; }
        public bool IsSafe { get; set; }
        public string Notes { get; set; }
        public byte[] ReleaseContent { get; set; }

        
        public virtual User User { get; set; }
    }
}