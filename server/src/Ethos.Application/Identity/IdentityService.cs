using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using AutoMapper;
using Ethos.Application.Contracts.Identity;
using Ethos.Application.Email;
using Ethos.Common;
using Ethos.Domain.Common;
using Ethos.Domain.Entities;
using Ethos.Domain.Exceptions;
using Ethos.Query.Services;
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
        private readonly IEmailSender _emailSender;
        private readonly IUserQueryService _userQueryService;
        private readonly IGuidGenerator _guidGenerator;
        private readonly IMapper _mapper;
        private readonly JwtConfig _jwtConfig;
        private readonly AppSettings _appSettings;

        /// <summary>
        /// Creates a new IdentityService.
        /// </summary>
        public IdentityService(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            SignInManager<ApplicationUser> signInManager,
            IOptions<JwtConfig> jwtConfigOptions,
            IEmailSender emailSender,
            IUserQueryService userQueryService,
            IGuidGenerator guidGenerator,
            IOptions<AppSettings> appSettingOptions,
            IMapper mapper)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _userQueryService = userQueryService;
            _guidGenerator = guidGenerator;
            _mapper = mapper;
            _jwtConfig = jwtConfigOptions.Value;
            _appSettings = appSettingOptions.Value;
        }

        /// <inheritdoc />
        public async Task CreateUserAsync(RegisterRequestDto input, string roleName)
        {
            if (await _userManager.FindByEmailAsync(input.Email) != null)
            {
                throw new BusinessException("User already existing");
            }

            if (await _userManager.FindByNameAsync(input.UserName) != null)
            {
                throw new BusinessException("User already existing");
            }

            var user = new ApplicationUser(_guidGenerator.Create(), input.Email, input.UserName, input.FullName);
            var result = await _userManager.CreateAsync(user, input.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(",", result.Errors.Select(e => e.Description));
                throw new BusinessException(errors);
            }

            await _userManager.AddToRoleAsync(user, roleName);
        }

        /// <inheritdoc />
        public async Task<LoginResponseDto> AuthenticateAsync(LoginRequestDto input)
        {
            var user = await _userManager.FindByNameAsync(input.UserNameOrEmail) ??
                       await _userManager.FindByEmailAsync(input.UserNameOrEmail);

            if (user == null)
            {
                throw new AuthenticationException("Invalid credentials!");
            }

            var result = await _signInManager.PasswordSignInAsync(
                user,
                input.Password,
                isPersistent: false,
                lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                throw new AuthenticationException("Invalid credentials!");
            }

            var userClaims = await _userManager.GetClaimsAsync(user);

            var identityOptions = new IdentityOptions();

            var claims = new List<Claim>
            {
                new Claim(identityOptions.ClaimsIdentity.UserIdClaimType, user.Id.ToString()),
                new Claim(identityOptions.ClaimsIdentity.UserNameClaimType, user.UserName),
            };

            var userRoles = await _userManager.GetRolesAsync(user);

            // the first role will be used to navigate in the UI to the main page for that role
            userRoles = userRoles.OrderBy(r => r).ToList();

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
                expires: DateTime.Now.AddMonths(1),
                signingCredentials: signingCredentials);

            var handler = new JwtSecurityTokenHandler();
            var tokenString = handler.WriteToken(token);

            return new LoginResponseDto()
            {
                AccessToken = tokenString,
                User = new UserDto()
                {
                    Id = user.Id,
                    Email = user.Email,
                    FullName = user.FullName,
                    UserName = user.UserName,
                    Roles = userRoles,
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

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            string message;
            var resourceName = "Ethos.Application.Email.Templates.reset-password.html";
            await using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            using (var reader = new StreamReader(stream!))
            {
                message = await reader.ReadToEndAsync();
            }

            var encodedToken = HttpUtility.UrlEncode(resetToken);
            var spaLink = $"{_appSettings.BaseUrl}/auth/reset-password?email={user.Email}&resetToken={encodedToken}";

            message = message.Replace("{{resetLink}}", spaLink);

            await _emailSender.SendEmail(
                recipient: user.Email,
                subject: "Ethos Training - Reset password",
                message);
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
                throw new BusinessException(errors);
            }
        }

        /// <inheritdoc />
        public async Task<IEnumerable<UserDto>> GetAllAdminsAsync()
        {
            var users = await _userQueryService.GetAllAdminsAsync();
            return users.Select(u => _mapper.Map<UserDto>(u));
        }
    }
}
