using System;
using System.ComponentModel.DataAnnotations.Schema;

using Cc.Upt.CommonDomain.Implementations;
using Cc.Upt.Domain.Enumerations;

namespace Cc.Upt.Domain
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