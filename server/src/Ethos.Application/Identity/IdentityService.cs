using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Ethos.Application.Contracts.Identity;
using Ethos.Application.Email;
using Ethos.Domain.Identity;
using Ethos.EntityFrameworkCore;
using Ethos.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly JwtConfig _jwtConfig;

        /// <summary>
        /// Creates a new IdentityService.
        /// </summary>
        public IdentityService(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            SignInManager<ApplicationUser> signInManager,
            IOptions<JwtConfig> jwtConfigOptions,
            IEmailSender emailSender,
            ApplicationDbContext applicationDbContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _applicationDbContext = applicationDbContext;
            _jwtConfig = jwtConfigOptions.Value;
        }

        /// <inheritdoc />
        public async Task CreateUserAsync(RegisterRequestDto input, string roleName)
        {
            var user = new ApplicationUser()
            {
                Email = input.Email,
                UserName = input.UserName,
                FullName = input.FullName,
            };

            var result = await _userManager.CreateAsync(user, input.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(",", result.Errors.Select(e => e.Description));
                throw new Exception(errors);
            }

            await _userManager.AddToRoleAsync(user, roleName);
        }

        /// <inheritdoc />
        public async Task<LoginResponseDto> AuthenticateAsync(LoginRequestDto input)
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
                User = new UserDto()
                {
                    Id = user.Id.ToString(),
                    Email = user.Email,
                    FullName = user.FullName,
                    UserName = user.UserName,
                },
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

        /// <inheritdoc />
        public async Task SendPasswordResetLinkAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return;
            }

            var resetLink = await _userManager.GeneratePasswordResetTokenAsync(user);

            await _emailSender.SendEmail(user.Email, "Reset Link", $"Reset the password using the link '{resetLink}'");
        }

        /// <inheritdoc />
        public async Task ResetPasswordAsync(ResetPasswordRequestDto input)
        {
            var user = await _userManager.FindByEmailAsync(input.Email);

            if (user == null)
            {
                return;
            }

            var result = await _userManager.ResetPasswordAsync(user, input.ResetToken, input.NewPassword);

            if (!result.Succeeded)
            {
                var errors = string.Join(",", result.Errors.Select(e => e.Description));
                throw new Exception(errors);
            }
        }

        /// <inheritdoc />
        public async Task<IEnumerable<UserDto>> GetUsersAsync()
        {
            var users = await _applicationDbContext.Users.ToListAsync();

            return users.Select(u => new UserDto()
            {
                Id = u.Id.ToString(),
                Email = u.Email,
                FullName = u.FullName,
                UserName = u.UserName,
            });
        }
    }
}
