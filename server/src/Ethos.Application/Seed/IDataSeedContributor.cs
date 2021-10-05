using System.Threading.Tasks;

namespace Ethos.Application.Seed
{
    /// <summary>
    /// Every class that has to seed some database data must implement this interface.
    /// </summary>
    public interface IDataSeedContributor
    {
        /// <summary>
        /// Seeds some data into the database.
        /// </summary>
        Task SeedAsync();
    }
}
