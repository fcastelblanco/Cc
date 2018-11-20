using System;

namespace Cc.Upt.Domain.Common.Definitions
{
    public interface IAuditableEntity 
    {
        Guid Id { get; set; }
        DateTime CreatedOn { get; set; } 
        string CreatedBy { get; set; }
        DateTime? UpdatedOn { get; set; }
        string UpdatedBy { get; set; }
    }
}
