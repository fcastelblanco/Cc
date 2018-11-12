using Cc.Upt.Business.Definitions;
using Cc.Upt.Domain.Dto;

namespace Cc.Upt.Business.Implementations.ValidateRules
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
