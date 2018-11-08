
using System.ComponentModel;

namespace Isn.Upt.Domain.Enumerations
{
    public enum Profile
    {
        [Description("Contacto de empresa")]
        CompanyContact,
        [Description("Creador de paquete")]
        PackageCreator,
        [Description("Administrador")]
        Administrator
    }
}
