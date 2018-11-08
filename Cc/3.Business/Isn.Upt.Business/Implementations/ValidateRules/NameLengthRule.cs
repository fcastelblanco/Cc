using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Isn.Upt.Business.Definitions;
using Isn.Upt.Domain.Dto;

namespace Isn.Upt.Business.Implementations.ValidateRules
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
