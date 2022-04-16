using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Ethos.Application.Contracts.Identity;
using Ethos.Application.Identity;
using Ethos.Common;
using Microsoft.AspNetCore.Mvc;

namespace Ethos.Web.Controllers
{
    /// <summary>
    ///     Manages all the operations on identity, such as user creation, authentication and role management.
    /// </summary>
    [Route("api/identity")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private readonly IIdentityService _identityService;

        public IdentityController(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        /// <summary>
        ///     Try to authenticate the given user.
        /// </summary>
        /// <param name="input">The user to authenticate.</param>
        /// <returns>The token or null.</returns>
        [HttpPost("authenticate")]
        public async Task<LoginResponseDto> AuthenticateAsync([Required] LoginRequestDto input)
        {
            return await _identityService.AuthenticateAsync(input);
        }

        /// <summary>
        ///     Create a new user in the system with the default role.
        /// </summary>
        /// <param name="input">The user to create.</param>
        [HttpPost("register")]
        public async Task RegisterUserAsync([Required] RegisterRequestDto input)
        {
            await _identityService.CreateUserAsync(input, RoleConstants.User);
        }

        /// <summary>
        ///     Send the password reset link.
        /// </summary>
        [HttpPost("send-password-reset-link")]
        public async Task SendPasswordResetLinkAsync([Required] string email)
        {
            await _identityService.SendPasswordResetLinkAsync(email);
        }

        /// <summary>
        ///     Reset the password using the reset link.
        /// </summary>
        [HttpPost("reset-password")]
        public async Task ResetPasswordAsync([Required] ResetPasswordRequestDto input)
        {
            await _identityService.ResetPasswordAsync(input);
        }

        /// <summary>
        ///     Return the list af all users with athe 'Admin' role.
        /// </summary>
        [Authorize]
        [HttpGet]
        public async Task<IEnumerable<UserDto>> GetAllAdminsAsync()
        {
            return await _identityService.GetAllAdminsAsync();
        }
        
        /// <summary>
        ///     Return the list af users that contains the given text.
        /// </summary>
        [Authorize(Roles = RoleConstants.Admin)]
        [HttpGet("search")]
        public async Task<IEnumerable<UserDto>> SearchUsersAsync([Required] string containsText)
        {
            return await _identityService.SearchUsersAsync(containsText);
        }
        
        /// <summary>
        ///     Add the 'Admin' role to the specified user.
        /// </summary>
        [Authorize(Roles = RoleConstants.Admin)]
        [HttpGet("add-role")]
        public async Task AddAdminRoleAsync([Required] Guid userId)
        {
            await _identityService.AddAdminRoleAsync(userId);
        }
    }
}
