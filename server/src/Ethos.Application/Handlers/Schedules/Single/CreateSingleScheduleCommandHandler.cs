using System;
using System.Threading;
using System.Threading.Tasks;
using Ethos.Application.Commands.Schedules.Single;
using Ethos.Application.Exceptions;
using Ethos.Common;
using Ethos.Domain.Common;
using Ethos.Domain.Entities;
using Ethos.Domain.Repositories;
using MediatR;
using Microsoft.AspNetCore.Identity;

#pragma warning disable CA1716

namespace Ethos.Application.Handlers.Schedules.Single
{
    public class CreateSingleScheduleCommandHandler : IRequestHandler<CreateSingleScheduleCommand, Guid>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IGuidGenerator _guidGenerator;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateSingleScheduleCommandHandler(
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

        public async Task<Guid> Handle(CreateSingleScheduleCommand request, CancellationToken cancellationToken)
        {
            var organizer = await _userManager.FindByIdAsync(request.OrganizerId.ToString());

            if (organizer == null || !await _userManager.IsInRoleAsync(organizer, RoleConstants.Admin))
            {
                throw new InvalidOrganizerException();
            }

            var schedule = SingleSchedule.Factory.Create(
                _guidGenerator.Create(),
                organizer,
                request.Name,
                request.Description,
                request.ParticipantsMaxNumber,
                request.StartDate,
                request.DurationInMinutes,
                TimeZoneInfo.FindSystemTimeZoneById(request.TimeZone));

            await _scheduleRepository.CreateAsync(schedule);

            await _unitOfWork.SaveChangesAsync();

            return schedule.Id;
        }
    }
}
