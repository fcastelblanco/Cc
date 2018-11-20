using Cc.Upt.Business.Definitions;
using Cc.Upt.Domain.DataTransferObject;


namespace Cc.Upt.Business.Implementations.ValidateRules
{
    public class BackUpRule : IValidateRule
    {
        public bool Validate(ValidateRuleDto model)
        {
            return model.BackUp ? model.Input.Contains("_BU") : true;
        }
    }
}
