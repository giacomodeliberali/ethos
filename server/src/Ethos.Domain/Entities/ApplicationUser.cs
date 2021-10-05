using System;
using Microsoft.AspNetCore.Identity;

namespace Ethos.Domain.Entities
{
    /// <summary>
    /// It represents a user which can access the application once it registers.
    /// </summary>
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string FullName { get; set; }
    }
}
