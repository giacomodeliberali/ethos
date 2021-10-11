using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Ethos.Domain.Entities;
using Ethos.Domain.Exceptions;
using Ethos.Domain.Repositories;
using Ethos.Shared;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Ethos.Application.Commands
{
    public class UpdateSingleScheduleCommandHandler : AsyncRequestHandler<UpdateSingleScheduleCommand>
    {
        private readonly IScheduleRepository _scheduleRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateSingleScheduleCommandHandler(
            IScheduleRepository scheduleRepository,
            UserManager<ApplicationUser> userManager,
            IUnitOfWork unitOfWork)
        {
            _scheduleRepository = scheduleRepository;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        protected override async Task Handle(UpdateSingleScheduleCommand request, CancellationToken cancellationToken)
        {
            var organizer = await _userManager.FindByIdAsync(request.Input.Schedule.OrganizerId.ToString());

            if (organizer == null || !await _userManager.IsInRoleAsync(organizer, RoleConstants.Admin))
            {
                throw new BusinessException("Invalid organizer id");
            }

            if (!string.IsNullOrEmpty(request.Input.Schedule.RecurringCronExpression))
            {
                Guard.Against.Default(request.Input.Schedule.StartDate, nameof(request.Input.Schedule.StartDate));

                // now it is recurring, create a new one with the same id
                await _scheduleRepository.DeleteAsync(request.Schedule);

                var newRecurring = RecurringSchedule.Factory.Create(
                    request.Schedule.Id,
                    organizer,
                    request.Input.Schedule.Name,
                    request.Input.Schedule.Description,
                    request.Input.Schedule.ParticipantsMaxNumber,
                    request.Input.Schedule.StartDate,
                    request.Input.Schedule.EndDate,
                    request.Input.Schedule.DurationInMinutes,
                    request.Input.Schedule.RecurringCronExpression);

                await _scheduleRepository.CreateAsync(newRecurring);
            }
            else
            {
                // just update
                request.Schedule.UpdateOrganizer(organizer);
                request.Schedule.UpdateNameAndDescription(request.Input.Schedule.Name, request.Input.Schedule.Description);
                request.Schedule.UpdateParticipantsMaxNumber(request.Input.Schedule.ParticipantsMaxNumber);

                Guard.Against.Default(request.Input.Schedule.StartDate, nameof(request.Input.Schedule.StartDate));
                Guard.Against.Null(request.Input.Schedule.EndDate, nameof(request.Input.Schedule.EndDate));

                request.Schedule.UpdateDateTime(request.Input.Schedule.StartDate, request.Input.Schedule.EndDate.Value);
                await _scheduleRepository.UpdateAsync(request.Schedule);
            }

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
