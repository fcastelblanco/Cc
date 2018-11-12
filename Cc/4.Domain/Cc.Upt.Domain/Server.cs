

using System;
using System.ComponentModel.DataAnnotations.Schema;

using Cc.Upt.CommonDomain.Implementations;


namespace Cc.Upt.Domain
{
    [Table("Server", Schema = "dbo")]
    public class Server : AuditableEntity<Guid>
    {
        public string Name { get; set; } // Name (length: 50)
        public int? Capacity { get; set; } // Capacity
        public int? Installed { get; set; } // Installed
        public bool? IsDataBaseServer { get; set; } // IsDataBaseServer
        public string UserName { get; set; } // UserName (length: 500)
        public string Password { get; set; } // Password (length: 500)
        public string Instance { get; set; } // Instance (length: 500)
        public string ServerIpOrName { get; set; } // ServerIpOrName (length: 500)
        public string Dns { get; set; } // Dns (length: 500)


        // Reverse navigation

        /// <summary>
        /// Child Companies where [Company].[ApplicationServerId] point to this entity (FK_Company_AppServer)
        /// </summary>
        [ForeignKey("ApplicationServerId")]
        public virtual System.Collections.Generic.ICollection<Company> ApplicationServerId { get; set; } // Company.FK_Company_AppServer

        /// <summary>
        /// Child Companies where [Company].[DataBaseServerId] point to this entity (FK_Company_DbServer)
        /// </summary>
        [ForeignKey("DataBaseServerId")]
        public virtual System.Collections.Generic.ICollection<Company> DataBaseServerId { get; set; } // Company.FK_Company_DbServer

        public Server()
        {
            Id = new Guid();
            ApplicationServerId = new System.Collections.Generic.List<Company>();
            DataBaseServerId = new System.Collections.Generic.List<Company>();
        }

    }

}
// </auto-generated>
