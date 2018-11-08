using Isn.Upt.Business.Definitions;
using Isn.Upt.Domain.Dto;

namespace Isn.Upt.Business.Implementations.ValidateRules
{
    public class ConsecutiveRule : IValidateRule
    {
        public bool Validate(ValidateRuleDto model)
        {
            var sections = model.Input.Split('_');
            if (sections.Length == 9)
            {
                var consecutive = 0;
                int.TryParse(sections[9], out consecutive);
                return consecutive > 0;
            }
            return false;
        }
    }
}
