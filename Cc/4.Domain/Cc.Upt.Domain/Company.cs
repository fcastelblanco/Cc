using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Cc.Common.Implementations;

namespace Cc.Upt.Domain
{
    [Table("Company", Schema = "dbo")]
    public class Company : AuditableEntity<Guid>
    {
        public Company()
        {
            Id = Guid.NewGuid();
            CompanyModules = new System.Collections.Generic.List<CompanyModule>();
        }

        [Required(AllowEmptyStrings = false, ErrorMessage = "El nombre es requerido")]
        public string Name { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Fecha final de soporte es requerida")]
        public DateTime DateEndSupport { get; set; }

        public string Prefix { get; set; } // Prefix (length: 500)
        public Guid? ApplicationServerId { get; set; } // ApplicationServerId
        public Guid? DataBaseServerId { get; set; } // DataBaseServerId
        public string Url { get; set; } // Url (length: 500)
        public Guid? UserRequestDeployId { get; set; } // UserRequestDeployId
        public DateTime? DeployDate { get; set; } // DeployDate
        public string ContactName { get; set; } // ContactName (length: 500)
        public string ContactPhone { get; set; } // ContactPhone (length: 500)
        public int? State { get; set; } // State
        public string ContactEmail { get; set; } // ContactEmail (length: 500)
        public int? Size { get; set; } // Size
        public int? SizeOnTestEnvironment { get; set; } // SizeOnTestEnvironment

        // Reverse navigation

        /// <summary>
        /// Child CompanyModules where [CompanyModule].[CompanyId] point to this entity (FK_CompanyModule_Company)
        /// </summary>
        public virtual System.Collections.Generic.ICollection<CompanyModule> CompanyModules { get; set; } // CompanyModule.FK_CompanyModule_Company

        // Foreign keys

        /// <summary>
        /// Parent Server pointed by [Company].([ApplicationServerId]) (FK_Company_AppServer)
        /// </summary>
        [ForeignKey("ApplicationServerId")]
        public virtual Server ApplicationServer { get; set; } // FK_Company_AppServer

        /// <summary>
        /// Parent Server pointed by [Company].([DataBaseServerId]) (FK_Company_DbServer)
        /// </summary>
        [ForeignKey("DataBaseServerId")]
        public virtual Server DataBaseServer { get; set; } // FK_Company_DbServer

        /// <summary>
        /// Parent User pointed by [Company].([UserRequestDeployId]) (FK_Company_User)
        /// </summary>

        public virtual User User { get; set; } // FK_Company_User
    }
}