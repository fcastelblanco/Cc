using System.ComponentModel;

namespace Cc.Upt.Domain.Enumerations
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
