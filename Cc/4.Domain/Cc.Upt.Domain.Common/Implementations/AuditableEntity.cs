using System;
using System.ComponentModel.DataAnnotations;
using Cc.Upt.Domain.Common.Definitions;

namespace Cc.Upt.Domain.Common.Implementations
{
    public abstract class AuditableEntity : IAuditableEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        [ScaffoldColumn(false)] public DateTime CreatedOn { get; set; } = DateTime.Now;
        [MaxLength(256)]
        [ScaffoldColumn(false)]
        public string CreatedBy { get; set; }
        [ScaffoldColumn(false)]
        public DateTime? UpdatedOn { get; set; } = DateTime.Now;
        [MaxLength(256)]
        [ScaffoldColumn(false)]
        public string UpdatedBy { get; set; }
    }
}