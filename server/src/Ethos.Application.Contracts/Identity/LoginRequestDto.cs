using System.ComponentModel.DataAnnotations;

namespace Ethos.Application.Contracts.Identity
{
    public class LoginRequestDto
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
