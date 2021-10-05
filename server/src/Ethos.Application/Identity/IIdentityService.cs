using System.Threading.Tasks;
using Ethos.Application.Contracts.Identity;
using Ethos.Domain.Identity;

namespace Ethos.Application.Identity
{
    /// <summary>
    /// The identity service.
    /// </summary>
    public interface IIdentityService
    {
        /// <summary>
        /// Creates a new user in the system with the specified role.
        /// </summary>
        Task CreateUserAsync(RegisterRequestDto input, string roleName);

        /// <summary>
        /// Tries to authenticate the given user.
        /// </summary>
        Task<LoginResponseDto> GetTokenAsync(LoginRequestDto input);

        /// <summary>
        /// Creates a new role with the given name.
        /// </summary>
        Task CreateRoleAsync(string name);

        /// <summary>
        /// Returns the given role or null.
        /// </summary>
        Task<ApplicationRole> GetRoleAsync(string name);

        /// <summary>
        /// Sends the password reset link.
        /// </summary>
        Task SendPasswordRecoveryLinkAsync(string email);

        /// <summary>
        /// Reset the password using the reset link.
        /// </summary>
        Task ResetPasswordAsync(ResetPasswordRequestDto input);
    }
}
