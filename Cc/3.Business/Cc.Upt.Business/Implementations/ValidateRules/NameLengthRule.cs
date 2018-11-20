using Cc.Upt.Business.Definitions;
using Cc.Upt.Domain.DataTransferObject;


namespace Cc.Upt.Business.Implementations.ValidateRules
{
    public class NameLengthRule : IValidateRule
    {
        public bool Validate(ValidateRuleDto model)
        {
            var lengthName = model.Input.Split('_');
            return lengthName.Length == model.LengthSplitUnderScore;
        }
    }
}
