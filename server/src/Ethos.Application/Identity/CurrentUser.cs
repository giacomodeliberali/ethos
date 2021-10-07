using System;
using System.Threading.Tasks;
using Ethos.Domain.Entities;
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
            var currentUserId = GetCurrentUserId();
            return await _userManager.FindByIdAsync(currentUserId.ToString());
        }

        public Guid GetCurrentUserId()
        {
            var claimsPrincipal = _httpContextAccessor.HttpContext.User;

            var claim = claimsPrincipal.FindFirst(c =>
            {
                return c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";
            });

            if (claim == null)
            {
                throw new Exception("Not authorized!");
            }

            return Guid.Parse(claim.Value);
        }
    }
}
