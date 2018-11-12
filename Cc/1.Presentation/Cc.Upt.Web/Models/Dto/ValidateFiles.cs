using Cc.Upt.Domain.Enumerations;

namespace Cc.Upt.Web.Models.Dto
{
    public class ValidateFiles
    {
        public FileTypeValidate FileTypeValidate { get; set; }
        public bool RequiredBackup { get; set; }
    }
}