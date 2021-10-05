using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Ethos.Application.Contracts;
using Ethos.Application.Contracts.Identity;
using Ethos.Application.Identity;
using Ethos.Domain.Identity;
using Ethos.EntityFrameworkCore;
using Ethos.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Ethos.Web.Controllers
{
    /// <summary>
    /// Manages all the operations on identity.
    /// </summary>
    [Route("api/accounts")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IIdentityService _identityService;

        /// <summary>
        /// Creates a new AccountController.
        /// </summary>
        public AccountsController(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        /// <summary>
        /// Tries to authenticate the given user.
        /// </summary>
        /// <param name="input">The user to authenticate.</param>
        /// <returns>The token or null.</returns>
        [HttpPost("authenticate")]
        public async Task<LoginResponseDto> GetAuthToken(LoginRequestDto input)
        {
            return await _identityService.GetTokenAsync(input);
        }

        /// <summary>
        /// Creates a new user in the system with the specified role.
        /// </summary>
        /// <param name="input">The user to create.</param>
        [HttpPost("register")]
        public async Task RegisterUser(RegisterRequestDto input)
        {
            await _identityService.CreateUserAsync(input, RoleConstants.Default);
        }
    }
}
