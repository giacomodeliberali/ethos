namespace Ethos.Application.Contracts.Identity
{
    /// <summary>
    /// The response for the login.
    /// </summary>
    public partial class LoginResponseDto
    {
        /// <summary>
        /// The granted Bearer token or null if authentication fails.
        /// </summary>
        public string AccessToken { get; set; }

        public UserDto User { get; set; }
    }
}
