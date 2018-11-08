using System;
using System.Text.RegularExpressions;
using Isn.Upt.Business.Definitions;
using Isn.Upt.Domain.Dto;

namespace Isn.Upt.Business.Implementations.ValidateRules
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
