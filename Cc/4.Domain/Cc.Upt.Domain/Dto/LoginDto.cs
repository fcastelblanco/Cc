using System.ComponentModel.DataAnnotations;

namespace Cc.Upt.Domain.Dto
{
    public class LoginDto
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "El nombre de usuario es requerido")]
        [Display(Name = "Usuario")]
        public string UserName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "La contrasenia es requerida")]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }
        [Display(Name = "Correo electrónico")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Verifique que sea una cuenta valida")]
        public string Email { get; set; }
    }
}
