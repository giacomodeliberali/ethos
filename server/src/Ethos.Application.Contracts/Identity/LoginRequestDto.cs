using System.ComponentModel.DataAnnotations;

namespace Ethos.Application.Contracts.Identity
{
    /// <summary>
    /// The login request dto.
    /// </summary>
    public class LoginRequestDto
    {
        /// <summary>
        /// The user name or email.
        /// </summary>
        [Required]
        public string UserNameOrEmail { get; set; }

        /// <summary>
        /// The user password.
        /// </summary>
        [Required]
        public string Password { get; set; }
    }
}
