using System;
using System.ComponentModel.DataAnnotations.Schema;
using Cc.Upt.Domain.Common.Implementations;

namespace Cc.Upt.Domain
{
    [Table("Company", Schema = "dbo")]
    public class Company : AuditableEntity
    {
        public string TaxId { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string Url { get; set; }
    }
}