﻿namespace Cc.Upt.Business.Definitions
{
    public interface IValidateStrategy
    {
        bool Validate(string input, bool backup);
    }
}