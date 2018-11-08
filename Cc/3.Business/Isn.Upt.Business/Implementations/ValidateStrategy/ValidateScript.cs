using System.Collections.Generic;
using System.Linq;
using Isn.Upt.Business.Definitions;
using Isn.Upt.Business.Implementations.ValidateRules;
using Isn.Upt.Domain.Dto;

namespace Isn.Upt.Business.Implementations.ValidateStrategy
{
    public class ValidateScript : IValidateStrategy
    {
        private readonly List<IValidateRule> _validateRules;

        public ValidateScript()
        {
            _validateRules = new List<IValidateRule>
            {
                { new  ExtensionRule()},
                { new  NameLengthRule()},
                { new  ConsecutiveRule()}
            };
        }
        public bool Validate(string input, bool backup)
        {
            var dto = new ValidateRuleDto()
            {
                Extension = "sql",
                Input = input,
                LengthSplitUnderScore = 9
            };
            return _validateRules.Aggregate(true, (currentResult, rule) => currentResult && rule.Validate(dto));
        }
    }
}