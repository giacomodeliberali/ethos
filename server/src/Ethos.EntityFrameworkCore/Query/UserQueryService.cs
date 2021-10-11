using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ethos.Domain.Entities;
using Ethos.Query.Projections;
using Ethos.Query.Services;
using Ethos.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Ethos.EntityFrameworkCore.Query
{
    public class UserQueryService : BaseQueryService, IUserQueryService
    {
        private IQueryable<ApplicationUser> UsersQueryable => ApplicationDbContext.Users.AsNoTracking();

        private IQueryable<IdentityUserRole<Guid>> UserRolesQueryable => ApplicationDbContext.UserRoles.AsNoTracking();

        private IQueryable<ApplicationRole> RolesQueryable => ApplicationDbContext.Roles.AsNoTracking();

        public UserQueryService(ApplicationDbContext applicationDbContext)
            : base(applicationDbContext)
        {
        }

        public async Task<IEnumerable<UserProjection>> GetAllAdminsAsync()
        {
            var users = await GetAllUsers();
            return users.Where(u => u.Roles.Contains(RoleConstants.Admin));
        }

        private async Task<IEnumerable<UserProjection>> GetAllUsers()
        {
            var users = await (
                from user in UsersQueryable
                join userRole in UserRolesQueryable on user.Id equals userRole.UserId
                join role in RolesQueryable on userRole.RoleId equals role.Id
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
