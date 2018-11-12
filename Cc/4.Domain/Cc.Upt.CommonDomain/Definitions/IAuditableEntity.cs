using System;

namespace Cc.Upt.CommonDomain.Definitions
{
    public interface IAuditableEntity 
    {
        DateTime CreatedDate { get; set; }
     
        string CreatedBy { get; set; }

        DateTime? UpdatedDate { get; set; }
             
        string UpdatedBy { get; set; }
    }
}
