using System;
using System.ComponentModel.DataAnnotations.Schema;
using Cc.Upt.Domain.Common.Implementations;
using Cc.Upt.Domain.Enumerations;

namespace Cc.Upt.Domain
{
    [Table("User", Schema = "dbo")]
    public class User : AuditableEntity
    {
        [ForeignKey("Company")] public Guid CompanyId { get; set; }

        public string Name { get; set; }
        public string LastName { get; set; }

        public string Password { get; set; }
        public Profile Profile { get; set; }
        public string Email { get; set; }

        public byte[] Photo { get; set; }
        public virtual Company Company { get; set; }

        [NotMapped]
        public bool IsAuthenticated { get; set; }
    }
}