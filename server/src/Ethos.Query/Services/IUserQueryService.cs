using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ethos.Query.Projections;

namespace Ethos.Query.Services
{
    public interface IUserQueryService
    {
        /// <summary>
        /// Return the list af all users with athe 'Admin' role.
        /// </summary>
        Task<IEnumerable<UserProjection>> GetAllAdminsAsync();
    }
}
