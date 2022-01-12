using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Ethos.Application.Commands;
using Ethos.Domain.Common;
using Ethos.Domain.Entities;
using Ethos.Domain.Exceptions;
using Ethos.Domain.Repositories;
using Ethos.Shared;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Ethos.Application.Handlers
{
    public class CreateScheduleCommandHandler : IRequestHandler<CreateScheduleCommand, Guid>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IGuidGenerator _guidGenerator;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateScheduleCommandHandler(
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

        public async Task<Guid> Handle(CreateScheduleCommand request, CancellationToken cancellationToken)
        {
            var organizer = await _userManager.FindByIdAsync(request.OrganizerId.ToString());

            if (organizer == null || !await _userManager.IsInRoleAsync(organizer, RoleConstants.Admin))
            {
                throw new BusinessException("Invalid organizer id");
            }

            var createdScheduleId = _guidGenerator.Create();
            if (string.IsNullOrEmpty(request.RecurringCronExpression))
            {
                var schedule = SingleSchedule.Factory.Create(
                    createdScheduleId,
                    organizer,
                    request.Name,
                    request.Description,
                    request.ParticipantsMaxNumber,
                    new Period(
                        request.StartDate,
                        request.StartDate.AddMinutes(request.DurationInMinutes)));

                await _scheduleRepository.CreateAsync(schedule);
            }
            else
            {
                Guard.Against.Null(request.EndDate, nameof(request.EndDate));

                var schedule = RecurringSchedule.Factory.Create(
                    createdScheduleId,
                    organizer,
                    request.Name,
                    request.Description,
                    request.ParticipantsMaxNumber,
                    new Period(request.StartDate, request.EndDate.Value),
                    request.DurationInMinutes,
                    request.RecurringCronExpression);

                await _scheduleRepository.CreateAsync(schedule);
            }

            await _unitOfWork.SaveChangesAsync();

            return createdScheduleId;
        }
    }
}
