using System.Collections.Generic;
using System.Threading.Tasks;
using Ethos.Query.Projections;

namespace Ethos.Query.Services
{
    public interface IUserQueryService
    {
        /// <summary>
        /// Return the list af all users with the 'Admin' role.
        /// </summary>
        Task<IEnumerable<UserProjection>> GetAllAdminsAsync();

        Task<IEnumerable<UserProjection>> SearchUsersAsync(string containsText);
    }
}
