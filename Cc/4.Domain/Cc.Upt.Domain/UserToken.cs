using System;
using System.ComponentModel.DataAnnotations.Schema;
using Cc.Upt.Domain.Common.Implementations;
using Cc.Upt.Domain.Enumerations;

namespace Cc.Upt.Domain
{
    [Table("UserToken", Schema = "dbo")]
    public class UserToken : AuditableEntity
    {
        public Guid UserId { get; set; }
        public Guid Token { get; set; }
        public DateTime Expiration { get; set; }
        public TokenType TokenType { get; set; }

        public virtual  User User { get; set; }
    }
}