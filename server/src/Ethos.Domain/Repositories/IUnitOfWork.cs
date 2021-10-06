using System.Threading.Tasks;

namespace Ethos.Domain.Repositories
{
    public interface IUnitOfWork
    {
        Task SaveChangesAsync();
    }
}
