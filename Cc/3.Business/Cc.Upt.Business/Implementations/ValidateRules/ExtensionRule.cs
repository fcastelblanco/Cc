using Cc.Upt.Business.Definitions;
using Cc.Upt.Domain.DataTransferObject;


namespace Cc.Upt.Business.Implementations.ValidateRules
{
    public class ExtensionRule: IValidateRule
    {
        public bool Validate(ValidateRuleDto model)
        {
            var name = model.Input.Split('.');
            if (name.Length != 2)
                return false;

            return name[1] == model.Extension;
        }
    }
}
