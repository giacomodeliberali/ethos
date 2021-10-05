using System;
using Microsoft.AspNetCore.Identity;

namespace Ethos.Domain.Entities
    {
    /// <summary>
    /// It represents role that can be assigned to an ApplicationUser.
    /// </summary>
    public class ApplicationRole : IdentityRole<Guid>
    {
    }
}
