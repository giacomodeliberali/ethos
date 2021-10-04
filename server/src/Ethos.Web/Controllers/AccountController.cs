using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Application.Identity;
using Ethos.Application.Contracts;
using Ethos.Application.Contracts.Identity;
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
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IIdentityService _identityService;

        public AccountController(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        [HttpPost("authenticate")]
        public async Task<string> GetAuthToken(LoginRequestDto input)
        {
            return await _identityService.GetTokenAsync(input);
        }

        [HttpPost("register")]
        public async Task RegisterUser(RegisterRequestDto input)
        {
            await _identityService.CreateUserAsync(input, RoleConstants.Default);
        }
    }
}
