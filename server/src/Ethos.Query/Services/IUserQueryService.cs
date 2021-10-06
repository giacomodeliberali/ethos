using System.Collections.Generic;
using System.Threading.Tasks;
using Ethos.Query.Projections;

namespace Ethos.Query.Services
{
    public interface IUserQueryService
    {
        Task<IEnumerable<UserProjection>> GetAllAsync();
    }
}
