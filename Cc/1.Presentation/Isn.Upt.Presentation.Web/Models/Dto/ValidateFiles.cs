using System;
using System.Collections.Generic;
using Isn.Upt.Domain.Enumerations;

namespace Isn.Upt.Presentation.Models.Dto
{
    public class ValidateFiles
    {
        public FileTypeValidate FileTypeValidate { get; set; }
        public bool RequiredBackup { get; set; }
    }
}