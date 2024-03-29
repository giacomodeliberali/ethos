using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ethos.Application.Contracts.Identity;
using Ethos.Domain.Entities;

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
        /// Creates a new user in the system with the specified roles.
        /// </summary>
        Task CreateUserAsync(RegisterRequestDto input, IEnumerable<string> roleNames);

        /// <summary>
        /// Tries to authenticate the given user.
        /// </summary>
        Task<LoginResponseDto> AuthenticateAsync(LoginRequestDto input);

        /// <summary>
        /// Creates a new role with the given name.
        /// </summary>
        Task CreateRoleAsync(string name);

        /// <summary>
        /// Returns the given role or null.
        /// </summary>
        Task<ApplicationRole?> GetRoleAsync(string name);

        /// <summary>
        /// Sends the password reset link.
        /// </summary>
        Task SendPasswordResetLinkAsync(string email);

        /// <summary>
        /// Reset the password using the reset link.
        /// </summary>
        Task ResetPasswordAsync(ResetPasswordRequestDto input);

        /// <summary>
        /// Return the list of all registered users.
        /// </summary>
        Task<IEnumerable<UserDto>> GetAllAdminsAsync();

        /// <summary>
        /// Return the list af users that starts with the given text.
        /// </summary>
        Task<IEnumerable<UserDto>> SearchUsersAsync(string containsText);

        /// <summary>
        /// Add the Admin role to the specified user.
        /// </summary>
        Task AddAdminRoleAsync(Guid userId);
    }
}
