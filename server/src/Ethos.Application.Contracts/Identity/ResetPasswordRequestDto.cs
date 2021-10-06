using System.ComponentModel.DataAnnotations;

namespace Ethos.Application.Contracts.Identity
{
    public class ResetPasswordRequestDto
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string NewPassword { get; set; }

        [Required]
        public string ResetToken { get; set; }
    }
}
