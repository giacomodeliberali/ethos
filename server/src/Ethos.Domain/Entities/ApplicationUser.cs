using System;
using Microsoft.AspNetCore.Identity;

namespace Ethos.Domain.Entities
{
    /// <summary>
    /// It represents a user which can access the application once it registers.
    /// </summary>
    public class ApplicationUser : IdentityUser<Guid>
    {
        public ApplicationUser(Guid id, string email, string userName, string fullName)
        {
            Id = id;
            Email = email;
            UserName = userName;
            FullName = fullName;
        }

        public string FullName { get; set; }
    }
}
