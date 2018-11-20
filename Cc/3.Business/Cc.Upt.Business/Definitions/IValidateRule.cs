using Cc.Upt.Domain.DataTransferObject;

namespace Cc.Upt.Business.Definitions
{
    public interface IValidateRule
    {
        bool Validate(ValidateRuleDto model);
    }
}