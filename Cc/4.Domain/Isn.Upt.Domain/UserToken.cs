using System;
using System.ComponentModel.DataAnnotations.Schema;
using Isn.Common.Implementations;
using Isn.Upt.Domain.Enumerations;

namespace Isn.Upt.Domain
{
    [Table("UserToken", Schema = "dbo")]
    public class UserToken : AuditableEntity<Guid>
    {
        public UserToken()
        {
            Id = Guid.NewGuid();
        }

        public Guid UserId { get; set; }
        public Guid Token { get; set; }
        public DateTime Expiration { get; set; }
        public TokenType TokenType { get; set; }
    }
}