using Isn.Upt.Business.Definitions;
using Isn.Upt.Domain.Dto;

namespace Isn.Upt.Business.Implementations.ValidateRules
{
    public class BackUpRule : IValidateRule
    {
        public bool Validate(ValidateRuleDto model)
        {
            return model.BackUp ? model.Input.Contains("_BU") : true;
        }
    }
}
