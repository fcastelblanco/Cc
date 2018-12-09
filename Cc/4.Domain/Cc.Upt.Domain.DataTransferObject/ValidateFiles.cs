using Cc.Upt.Domain.Enumerations;

namespace Cc.Upt.Domain.DataTransferObject
{
    public class ValidateFiles
    {
        public FileTypeValidate FileTypeValidate { get; set; }
        public bool RequiredBackup { get; set; }
    }
}