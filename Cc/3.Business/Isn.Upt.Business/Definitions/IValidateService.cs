using Isn.Upt.Domain.Enumerations;

namespace Isn.Upt.Business.Definitions
{
    public interface IValidateService
    {
        bool Validate(FileTypeValidate fileType, string input, bool backup);
    }
}