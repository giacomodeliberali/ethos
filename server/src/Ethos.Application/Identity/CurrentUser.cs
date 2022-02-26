using System;
using System.Threading.Tasks;
using Ethos.Domain.Entities;
using Ethos.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Ethos.Application.Identity
{
    public class CurrentUser : ICurrentUser
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;

        public CurrentUser(
            IHttpContextAccessor httpContextAccessor,
            UserManager<ApplicationUser> userManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        public async Task<ApplicationUser> GetCurrentUser()
        {
            var currentUserId = UserId();
            return await _userManager.FindByIdAsync(currentUserId.ToString());
        }

        public Guid UserId()
        {
            var claimsPrincipal = _httpContextAccessor.HttpContext!.User;

            var claim = claimsPrincipal.FindFirst(c =>
            {
                return c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";
            });

            if (claim == null)
            {
                throw new AuthenticationException("Not authorized!");
            }

            return Guid.Parse(claim.Value);
        }

        public async Task<bool> IsInRole(string role)
        {
            var currentUser = await GetCurrentUser();
            return await _userManager.IsInRoleAsync(currentUser, role);
        }
    }
}
