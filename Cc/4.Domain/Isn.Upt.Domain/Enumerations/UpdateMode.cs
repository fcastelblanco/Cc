using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Isn.Upt.Domain.Enumerations
{
    public enum UpdateMode
    {
        [Description("Fuera de línea")]
        OffLine,
        [Description("En línea")]
        OnLine
    }
}
