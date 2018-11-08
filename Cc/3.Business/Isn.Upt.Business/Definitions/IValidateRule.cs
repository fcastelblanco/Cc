using Isn.Upt.Domain.Dto;

namespace Isn.Upt.Business.Definitions
{
    public interface IValidateRule
    {
        bool Validate(ValidateRuleDto model);
    }
}