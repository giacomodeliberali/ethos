using Ethos.Domain.Common;
using Ethos.Domain.Repositories;

namespace Ethos.Application.Services
{
    public class BaseApplicationService
    {
        protected IUnitOfWork UnitOfWork { get; }

        protected IGuidGenerator GuidGenerator { get; }

        protected BaseApplicationService(
            IUnitOfWork unitOfWork,
            IGuidGenerator guidGenerator)
        {
            UnitOfWork = unitOfWork;
            GuidGenerator = guidGenerator;
        }
    }
}
