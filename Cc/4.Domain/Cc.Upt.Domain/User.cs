using System;
using System.ComponentModel.DataAnnotations.Schema;
using Cc.Common.Implementations;
using Cc.Upt.Domain.Enumerations;

namespace Cc.Upt.Domain
{
    [Table("User", Schema = "dbo")]
    public class User : AuditableEntity<Guid>
    {
        public User()
        {
            Companies = new System.Collections.Generic.List<Company>();
        }
        
        public string Name { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public Profile Profile { get; set; }
        public string Email { get; set; }
        public Guid CompanyId { get; set; }
        public byte[] Photo { get; set; }

        [ForeignKey("UserRequestDeployId")]
        public virtual System.Collections.Generic.ICollection<Company> Companies { get; set; } // Company.FK_Company_User
    }
}