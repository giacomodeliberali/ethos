using System;
using System.Threading;
using System.Threading.Tasks;
using Ethos.Application.Commands.Schedules.Recurring;
using Ethos.Application.Exceptions;
using Ethos.Common;
using Ethos.Domain.Common;
using Ethos.Domain.Entities;
using Ethos.Domain.Repositories;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Ethos.Application.Handlers.Schedules.Recurring
{
    public class CreateRecurringScheduleCommandHandler : IRequestHandler<CreateRecurringScheduleCommand, Guid>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IGuidGenerator _guidGenerator;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateRecurringScheduleCommandHandler(
            UserManager<ApplicationUser> userManager,
            IGuidGenerator guidGenerator,
            IScheduleRepository scheduleRepository,
            IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _guidGenerator = guidGenerator;
            _scheduleRepository = scheduleRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(CreateRecurringScheduleCommand request, CancellationToken cancellationToken)
        {
            var organizer = await _userManager.FindByIdAsync(request.OrganizerId.ToString());

            if (organizer == null || !await _userManager.IsInRoleAsync(organizer, RoleConstants.Admin))
            {
                throw new InvalidOrganizerException();
            }

            var schedule = RecurringSchedule.Factory.Create(
                _guidGenerator.Create(),
                organizer,
                request.Name,
                request.Description,
                request.ParticipantsMaxNumber,
                new Period(request.StartDate, request.EndDate),
                request.DurationInMinutes,
                request.RecurringCronExpression);

            await _scheduleRepository.CreateAsync(schedule);

            await _unitOfWork.SaveChangesAsync();

            return schedule.Id;
        }
    }
}
