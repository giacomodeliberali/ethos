using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Ethos.Application.Contracts.Identity
{
    /// <summary>
    /// The response for the login.
    /// </summary>
    [KnownType(typeof(ExceptionDto))]
    public class LoginResponseDto
    {
        /// <summary>
        /// The granted Bearer token or null if authentication fails.
        /// </summary>
        [Required]
        public string AccessToken { get; set; }

        [Required]
        public UserDto User { get; set; }
    }
}
