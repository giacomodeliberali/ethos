using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Ethos.Application.Contracts.Identity;
using Ethos.Domain.Identity;
using Ethos.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Ethos.Application.Identity
{
    /// <inheritdoc />
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly JwtConfig _jwtConfig;

        /// <summary>
        /// Creates a new IdentityService.
        /// </summary>
        public IdentityService(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            SignInManager<ApplicationUser> signInManager,
            IOptions<JwtConfig> jwtConfigOptions)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _jwtConfig = jwtConfigOptions.Value;
        }

        /// <inheritdoc />
        public async Task CreateUserAsync(RegisterRequestDto input, string roleName)
        {
            var user = new ApplicationUser()
            {
                Email = input.Email,
                UserName = input.UserName,
            };

            var result = await _userManager.CreateAsync(user, input.Password);

            if (result.Succeeded)
            {
                var role = new ApplicationRole()
                {
                    Name = roleName,
                };

                await _roleManager.CreateAsync(role);

                await _userManager.AddToRoleAsync(user, roleName);
            }

            var errors = string.Join(",", result.Errors.Select(e => e.Description));
            throw new Exception(errors);
        }

        /// <inheritdoc />
        public async Task<LoginResponseDto> GetTokenAsync(LoginRequestDto input)
        {
            var result = await _signInManager.PasswordSignInAsync(
                input.UserName,
                input.Password,
                false,
                false);

            if (!result.Succeeded)
            {
                return null;
            }

            var user = await _userManager.FindByNameAsync(input.UserName);

            var userClaims = await _userManager.GetClaimsAsync(user);

            var identityOptions = new IdentityOptions();

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, "api"), // TODO @GDL from configuration?
                new Claim(identityOptions.ClaimsIdentity.UserIdClaimType, user.Id.ToString()),
                new Claim(identityOptions.ClaimsIdentity.UserNameClaimType, user.UserName),
            };

            var userRoles = await _userManager.GetRolesAsync(user);

            claims.AddRange(userClaims);

            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole));
                var role = await _roleManager.FindByNameAsync(userRole);
                if (role != null)
                {
                    var roleClaims = await _roleManager.GetClaimsAsync(role);
                    foreach (var roleClaim in roleClaims)
                    {
                        claims.Add(roleClaim);
                    }
                }
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Secret));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtConfig.TokenIssuer,
                audience: _jwtConfig.ValidAudience,
                claims: claims,
                notBefore: new DateTime(2021, 10, 1),
                expires: DateTime.Now.AddDays(1),
                signingCredentials: signingCredentials);

            var handler = new JwtSecurityTokenHandler();
            var tokenString = handler.WriteToken(token);

            return new LoginResponseDto()
            {
                AccessToken = tokenString,
            };
        }

        /// <inheritdoc />
        public async Task CreateRoleAsync(string name)
        {
            var role = new ApplicationRole()
            {
                Name = name,
            };
            await _roleManager.CreateAsync(role);
        }

        /// <inheritdoc />
        public async Task<ApplicationRole> GetRoleAsync(string name)
        {
            return await _roleManager.FindByNameAsync(name);
        }
    }
}
