using System;
using System.ComponentModel.DataAnnotations.Schema;
using Isn.Common.Implementations;
using Isn.Upt.Domain.Enumerations;

namespace Isn.Upt.Domain
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