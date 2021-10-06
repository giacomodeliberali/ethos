using Ethos.Domain.Repositories;

namespace Ethos.Application.Services
{
    public class BaseApplicationService
    {
        protected IUnitOfWork UnitOfWork { get; }

        public BaseApplicationService(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }
    }
}
