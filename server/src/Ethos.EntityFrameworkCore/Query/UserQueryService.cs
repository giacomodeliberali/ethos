using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ethos.Query.Projections;
using Ethos.Query.Services;
using Microsoft.EntityFrameworkCore;

namespace Ethos.EntityFrameworkCore.Query
{
    public class UserQueryService : BaseQueryService, IUserQueryService
    {
        public UserQueryService(ApplicationDbContext applicationDbContext)
            : base(applicationDbContext)
        {
        }

        public async Task<IEnumerable<UserProjection>> GetAllAsync()
        {
            var users =
                await (from user in ApplicationDbContext.Users.AsNoTracking()
                    join userRole in ApplicationDbContext.UserRoles.AsNoTracking() on user.Id equals userRole.UserId
                    join role in ApplicationDbContext.Roles.AsNoTracking() on userRole.RoleId equals role.Id
                    select new { User = user, Role = role }).ToListAsync();

            var distinctUsers = users
                .GroupBy(i => i.User.Id)
                .Select(g => new
                {
                    User = g.First().User,
                    Roles = g.Select(gg => gg.Role),
                });

            return distinctUsers.Select(item => new UserProjection()
            {
                Id = item.User.Id,
                Email = item.User.Email,
                FullName = item.User.FullName,
                UserName = item.User.UserName,
                Roles = item.Roles.Select(r => r.Name).ToList(),
            });
        }
    }
}
