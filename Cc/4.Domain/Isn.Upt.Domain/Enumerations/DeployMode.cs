using System.ComponentModel;

namespace Isn.Upt.Domain.Enumerations
{
    public enum DeployMode
    {
        [Description("Una sola empresa")] One,
        [Description("Muchas empresas")] Multiple
    }
}