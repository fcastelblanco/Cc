using System;
using System.ComponentModel.DataAnnotations.Schema;
using Cc.Common.Implementations;
using Cc.Upt.Domain.Enumerations;

namespace Cc.Upt.Domain
{
    [Table("Parameter", Schema = "dbo")]
    public class Parameter : AuditableEntity<Guid>
    {
        public Parameter()
        {
            Id = Guid.NewGuid();
        }

        public  ParameterInternalIdentificator ParameterInternalIdentificator { get; set; }
        public string Value { get; set; }
        public ParameterType ParameterType { get; set; }
    }
}