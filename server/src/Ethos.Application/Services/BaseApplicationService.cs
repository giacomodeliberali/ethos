using Ethos.Domain.Common;
using Ethos.Domain.Repositories;
using MediatR;

namespace Ethos.Application.Services
{
    public class BaseApplicationService
    {
        protected IMediator Mediator { get; }

        protected IUnitOfWork UnitOfWork { get; }

        protected BaseApplicationService(
            IMediator mediator,
            IUnitOfWork unitOfWork)
        {
            Mediator = mediator;
            UnitOfWork = unitOfWork;
        }
    }
}
