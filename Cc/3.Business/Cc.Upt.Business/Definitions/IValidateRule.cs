using Cc.Upt.Domain.Dto;

namespace Cc.Upt.Business.Definitions
{
    public interface IValidateRule
    {
        bool Validate(ValidateRuleDto model);
    }
}