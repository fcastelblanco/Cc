﻿using System.Collections.Generic;
using System.Linq;
using Cc.Upt.Business.Definitions;
using Cc.Upt.Business.Implementations.ValidateRules;
using Cc.Upt.Domain.DataTransferObject;

namespace Cc.Upt.Business.Implementations.ValidateStrategy
{
    public class AttachedBankValidateFolder : IValidateStrategy
    {
        private readonly List<IValidateRule> _validateRules;

        public AttachedBankValidateFolder()
        {
            _validateRules = new List<IValidateRule>
            {
                { new  NameRule()},
                { new  ExtensionRule()},
                { new  NameLengthRule()},
                { new  BackUpRule()}
            };
        }
        public bool Validate(string input, bool backup)
        {
            var dto = new ValidateRuleDto()
            {
                NameFile = "BancoAnexo",
                Extension = "zip",
                Input = input,
                LengthSplitUnderScore = 1,
                BackUp = backup
            };
            return _validateRules.Aggregate(true, (currentResult, rule) => currentResult && rule.Validate(dto));
        }
    }
}
