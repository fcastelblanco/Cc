using System.Collections.Generic;
using System.Linq;
using Cc.Upt.Business.Definitions;
using Cc.Upt.Business.Implementations.ValidateRules;
using Cc.Upt.Domain.Dto;

namespace Cc.Upt.Business.Implementations.ValidateStrategy
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