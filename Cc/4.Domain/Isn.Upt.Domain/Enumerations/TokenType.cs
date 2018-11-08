using System.ComponentModel;

namespace Isn.Upt.Domain.Enumerations
{
    public enum TokenType
    {
        [Description("Creación de usuario")] CreateUser,
        [Description("Olvido de contraseña")] ForgotPassword
    }
}