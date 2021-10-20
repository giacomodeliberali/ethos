using System;
using System.Threading.Tasks;
using Ethos.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Ethos.Domain.Extensions
{
    public static class UserManagerExtensions
    {
        /// <summary>
        /// Finds and returns a user, if any, who has the specified <paramref name="userId"/>.
        /// </summary>
        /// <param name="userManager">The user manager.</param>
        /// <param name="userId">The user ID to search for.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, containing the user matching the specified <paramref name="userId"/> if it exists.
        /// </returns>
        public static Task<ApplicationUser> FindByIdAsync(this UserManager<ApplicationUser> userManager, Guid userId)
        {
            return userManager.FindByIdAsync(userId.ToString());
        }
    }
}
