﻿using System;
using System.ComponentModel.DataAnnotations.Schema;
using Isn.Common.Implementations;

namespace Isn.Upt.Domain
{
    [Table("CompanyUpdate", Schema = "dbo")]
    public sealed class CompanyUpdate : AuditableEntity<Guid>
    {
        public Guid ReleaseId { get; set; }
        public DateTime Update { get; set; }
        public Guid CompanyId { get; set; }

        public CompanyUpdate()
        {
            Id = Guid.NewGuid();
        }
    }
}