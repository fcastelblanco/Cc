﻿using System.ComponentModel;

namespace Cc.Upt.Domain.Enumerations
{
    public enum ParameterStatus
    {
        [Description("No existe")] DoesNotExist,
        [Description("Nulo o vacío")] NullOrEmpty,
        [Description("Existe")] Exist
    }
}