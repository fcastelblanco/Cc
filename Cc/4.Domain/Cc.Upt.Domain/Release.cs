using System;
using System.ComponentModel.DataAnnotations.Schema;
using Cc.Common.Implementations;

namespace Cc.Upt.Domain
{
    [Table("Release", Schema = "dbo")]
    public sealed class Release : AuditableEntity<Guid>
    {
        public string Description { get; set; }
        public string Version { get; set; }
        public Guid UserId { get; set; }
        public DateTime Published { get; set; }
        public bool IsSafe { get; set; }
        public string Notes { get; set; }
        public byte[] ReleaseContent { get; set; }

        public Release()
        {
            Id = Guid.NewGuid();
        }
    }
}