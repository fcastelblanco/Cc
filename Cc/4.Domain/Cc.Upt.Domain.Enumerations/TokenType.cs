using System.ComponentModel;

namespace Cc.Upt.Domain.Enumerations
{
    public enum TokenType
    {
        [Description("Creación de usuario")] CreateUser,
        [Description("Olvido de contraseña")] ForgotPassword
    }
}