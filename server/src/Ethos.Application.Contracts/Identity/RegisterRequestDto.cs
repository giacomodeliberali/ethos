using System.ComponentModel.DataAnnotations;

namespace Ethos.Application.Contracts.Identity
{
    /// <summary>
    /// The DTO for creating a new user.
    /// </summary>
    public class RegisterRequestDto
    {
        /// <summary>
        /// The email address.
        /// </summary>
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        /// <summary>
        /// The user name (not email).
        /// </summary>
        [Required]
        [MinLength(5)]
        public string UserName { get; set; }

        /// <summary>
        /// The first and last name.
        /// </summary>
        [Required]
        [MinLength(5)]
        public string FullName { get; set; }

        /// <summary>
        /// The user password.
        /// </summary>
        [Required]
        public string Password { get; set; }
    }
}
