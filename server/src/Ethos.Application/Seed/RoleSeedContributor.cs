using System.Linq;
using System.Threading.Tasks;
using Ethos.Application.Contracts.Identity;
using Ethos.Application.Identity;
using Ethos.Shared;

namespace Ethos.Application.Seed
{
    /// <summary>
    /// Seed the default roles and the admin user.
    /// </summary>
    public class RoleSeedContributor : IDataSeedContributor
    {
        private readonly IIdentityService _identityService;

        public RoleSeedContributor(
            IIdentityService identityService)
        {
            _identityService = identityService;
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
            if (await _identityService.GetRoleAsync(RoleConstants.User) == null)
            {
                await _identityService.CreateRoleAsync(RoleConstants.User);
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
                    FullName = "Amministratore",
                    UserName = "admin",
                }, RoleConstants.Admin);
            }
        }
    }
}
