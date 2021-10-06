using Ethos.Domain.Repositories;

namespace Ethos.Application.Services
{
    public class BaseApplicationService
    {
        protected IUnitOfWork UnitOfWork { get; }

        protected BaseApplicationService(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }
    }
}
