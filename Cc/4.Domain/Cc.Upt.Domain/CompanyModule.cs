
using System;
using System.ComponentModel.DataAnnotations.Schema;
using Cc.Common.Implementations;

#pragma warning disable 1591    //  Ignore "Missing XML Comment" warning


namespace Cc.Upt.Domain
{

    [Table("CompanyModule", Schema = "dbo")]
    public class CompanyModule : AuditableEntity<Guid>
    {

        public System.Guid? CompanyId { get; set; } // CompanyId
        public System.Guid? ModuleId { get; set; } // ModuleId

        public CompanyModule()
        {
            Id = new Guid();
        }

        // Foreign keys

        /// <summary>
        /// Parent Company pointed by [CompanyModule].([CompanyId]) (FK_CompanyModule_Company)
        /// </summary>
        public virtual Company Company { get; set; } // FK_CompanyModule_Company

        /// <summary>
        /// Parent Module pointed by [CompanyModule].([ModuleId]) (FK_CompanyModule_Module)
        /// </summary>
        public virtual Module Module { get; set; } // FK_CompanyModule_Module
    }

}
// </auto-generated>
