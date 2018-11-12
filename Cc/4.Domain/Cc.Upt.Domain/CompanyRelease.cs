using System;
using System.ComponentModel.DataAnnotations.Schema;
using Cc.Common.Implementations;

namespace Cc.Upt.Domain
{
    [Table("CompanyRelease", Schema = "dbo")]
    public sealed class CompanyRelease : AuditableEntity<Guid>
    {
        public Guid ReleaseId { get; set; }
        public Guid CompanyId { get; set; }
        public CompanyRelease()
        {
            Id = Guid.NewGuid();
        }
    }
}