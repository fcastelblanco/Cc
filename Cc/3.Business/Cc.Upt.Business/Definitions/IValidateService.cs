using Cc.Upt.Domain.Enumerations;

namespace Cc.Upt.Business.Definitions
{
    public interface IValidateService
    {
        bool Validate(FileTypeValidate fileType, string input, bool backup);
    }
}