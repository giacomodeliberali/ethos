using System.Linq;
using System.Threading.Tasks;
using Ethos.Application.Contracts.Identity;
using Ethos.Application.Identity;
using Ethos.EntityFrameworkCore;
using Ethos.Shared;
using Microsoft.EntityFrameworkCore;

namespace Ethos.Application.Seed
{
    /// <summary>
    /// Seed the default roles.
    /// </summary>
    public class RoleSeedContributor : IDataSeedContributor
    {
        private readonly IIdentityService _identityService;
        private readonly ApplicationDbContext _applicationDbContext;

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public RoleSeedContributor(
            IIdentityService identityService,
            ApplicationDbContext applicationDbContext)
        {
            _identityService = identityService;
            _applicationDbContext = applicationDbContext;
        }

        /// <inheritdoc />
        public async Task SeedAsync()
        {
            // seed admin
            if (await _identityService.GetRoleAsync(RoleConstants.Admin) == null)
            {
                await _identityService.CreateRoleAsync(RoleConstants.Admin);
            }

            // seed default
            if (await _identityService.GetRoleAsync(RoleConstants.Default) == null)
            {
                await _identityService.CreateRoleAsync(RoleConstants.Default);
            }

            // seed default admin, change password in UI
            var users = await _identityService.GetUsersAsync();
            if (!users.Any())
            {
                await _identityService.CreateUserAsync(
                    new RegisterRequestDto()
                {
                    Email = "admin@ethos.it",
                    Password = "P2ssw0rd!",
                    ConfirmPassword = "P2ssw0rd!",
                    FullName = "Amministratore",
                    UserName = "admin",
                }, RoleConstants.Admin);
            }
        }
    }
}
