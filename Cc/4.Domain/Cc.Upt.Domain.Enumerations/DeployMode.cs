using System.ComponentModel;

namespace Cc.Upt.Domain.Enumerations
{
    public enum DeployMode
    {
        [Description("Una sola empresa")] One,
        [Description("Muchas empresas")] Multiple
    }
}