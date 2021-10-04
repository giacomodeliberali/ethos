namespace Ethos.Domain.Identity
{
    using System;
    using Microsoft.AspNetCore.Identity;

    /// <summary>
    /// It represents a user which can access the application once it registers.
    /// </summary>
    public class ApplicationUser : IdentityUser<Guid>
    {
    }
}
