using System;

namespace Isn.Upt.Domain.Dto
{
    public class SavePasswordDto
    {
        public Guid UserId { get; set; }
        public string Password { get; set; }
        public string ConfirmationPassword { get; set; }
    }
}