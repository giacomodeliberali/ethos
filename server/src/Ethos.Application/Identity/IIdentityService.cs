using System.Threading.Tasks;
using Ethos.Application.Contracts.Identity;

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
    }
}
