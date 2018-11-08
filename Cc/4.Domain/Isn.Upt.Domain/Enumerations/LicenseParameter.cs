﻿using System.ComponentModel;

namespace Isn.Upt.Domain.Enumerations
{
    public enum LicenseParameter
    {
        [Description("MOTOR_BASEDATOS")] DataBaseEngine,
        [Description("CADENA_CONEXION_SF")] ConnectionStringSf,
        [Description("SCHEMA_NAME")] SchemaName
    }
}