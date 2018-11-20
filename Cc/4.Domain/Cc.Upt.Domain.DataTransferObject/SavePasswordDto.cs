using System;

namespace Cc.Upt.Domain.DataTransferObject
{
    public class SavePasswordDto
    {
        public Guid UserId { get; set; }
        public string Password { get; set; }
        public string ConfirmationPassword { get; set; }
    }
}