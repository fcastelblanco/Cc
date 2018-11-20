using System.ComponentModel.DataAnnotations;

namespace Cc.Upt.Domain.DataTransferObject
{
    public class LoginDto
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "La contrasenia es requerida")]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }
        [Display(Name = "Correo electrónico")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Verifique que sea una cuenta valida")]
        public string Email { get; set; }
    }
}
