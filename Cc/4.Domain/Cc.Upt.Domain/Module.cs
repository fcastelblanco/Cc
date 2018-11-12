

using System;
using System.ComponentModel.DataAnnotations.Schema;
using Cc.Upt.CommonDomain.Implementations;


namespace Cc.Upt.Domain
{
    [Table("Module", Schema = "dbo")]
    public class Module : AuditableEntity<Guid>
    {
        public string Name { get; set; } // Name (length: 500)
        public int? IsolucionId { get; set; } // IsolucionId
        public string ProcedureParameterName { get; set; } // ProcedureParameterName (length: 500)
                                                           // Reverse navigation

        /// <summary>
        /// Child CompanyModules where [CompanyModule].[ModuleId] point to this entity (FK_CompanyModule_Module)
        /// </summary>
        public virtual System.Collections.Generic.ICollection<CompanyModule> CompanyModules { get; set; } // CompanyModule.FK_CompanyModule_Module
        public Module()
        {
            Id = new Guid();
            CompanyModules = new System.Collections.Generic.List<CompanyModule>();
        }
    }

}
// </auto-generated>
