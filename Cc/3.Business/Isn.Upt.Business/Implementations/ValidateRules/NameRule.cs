using Isn.Upt.Business.Definitions;
using Isn.Upt.Domain.Dto;

namespace Isn.Upt.Business.Implementations.ValidateRules
{
    public class NameRule : IValidateRule
    {
        public bool Validate(ValidateRuleDto model)
        {
            var name = model.Input.Split('.');
            if (name.Length != 2)
                return false;

            return name[0].ToUpper() == model.NameFile.ToUpper();
        }
    }
}