using System.ComponentModel.DataAnnotations.Schema;
using Cc.Upt.Domain.Common.Implementations;

namespace Cc.Upt.Domain
{
    [Table("Server", Schema = "dbo")]
    public class Server : AuditableEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }
}