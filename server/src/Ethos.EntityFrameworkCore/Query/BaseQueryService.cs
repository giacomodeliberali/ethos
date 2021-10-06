namespace Ethos.EntityFrameworkCore.Query
{
    public abstract class BaseQueryService
    {
        protected ApplicationDbContext ApplicationDbContext { get; }

        protected BaseQueryService(ApplicationDbContext applicationDbContext)
        {
            ApplicationDbContext = applicationDbContext;
        }
    }
}
