using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Ethos.Application.Contracts.Schedule;
using Ethos.Domain.Common;
using Ethos.Domain.Entities;
using Ethos.Domain.Exceptions;
using Ethos.Domain.Repositories;
using Ethos.Shared;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Ethos.Application.Commands
{
    public class UpdateRecurringScheduleCommandHandler : AsyncRequestHandler<UpdateRecurringScheduleCommand>
    {
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IScheduleExceptionRepository _scheduleExceptionRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IGuidGenerator _guidGenerator;

        public UpdateRecurringScheduleCommandHandler(
            IScheduleRepository scheduleRepository,
            IScheduleExceptionRepository scheduleExceptionRepository,
            UserManager<ApplicationUser> userManager,
            IGuidGenerator guidGenerator)
        {
            _scheduleRepository = scheduleRepository;
            _scheduleExceptionRepository = scheduleExceptionRepository;
            _userManager = userManager;
            _guidGenerator = guidGenerator;
        }

        protected override async Task Handle(UpdateRecurringScheduleCommand request, CancellationToken cancellationToken)
        {
            Guard.Against.Default(request.Input.InstanceStartDate, nameof(request.Input.InstanceStartDate));
            Guard.Against.Default(request.Input.InstanceEndDate, nameof(request.Input.InstanceEndDate));

            var occurrences = request.Schedule.RecurringCronExpression.GetOccurrences(
                request.Input.InstanceStartDate.Value,
                request.Input.InstanceEndDate.Value,
                fromInclusive: true,
                toInclusive: true).ToList();

            if (occurrences.Count != 1 || occurrences.Single() < request.Schedule.Period.StartDate || occurrences.Single() > request.Schedule.Period.EndDate)
            {
                throw new BusinessException("Invalid instance start/end dates");
            }

            var organizer = await _userManager.FindByIdAsync(request.Input.Schedule.OrganizerId.ToString());

            if (organizer == null || !await _userManager.IsInRoleAsync(organizer, RoleConstants.Admin))
            {
                throw new BusinessException("Invalid organizer id");
            }

            if (string.IsNullOrEmpty(request.Input.Schedule.RecurringCronExpression))
            {
                // it was recurring, now it is single
                // devo terminare lo scheduling passato ad oggi e creare un nuovo scheduling futuro singolo con nuova start date. Le prenotazioni future vanno cancellate
                await RecurringToSingle(request, organizer);
            }
            else
            {
                Guard.Against.Null(request.Input.RecurringScheduleOperationType, nameof(request.Input.RecurringScheduleOperationType));

                switch (request.Input.RecurringScheduleOperationType)
                {
                    case RecurringScheduleOperationType.Future:
                        // devo terminare lo scheduling passato ad oggi e creare un nuovo scheduling futuro che parte dalla nuova start date. Le prenotazioni future vanno cancellate
                        await RecurringToRecurring_UpdateInstanceAndFutures(request, organizer);
                        break;
                    case RecurringScheduleOperationType.Instance:
                        // aggiungo alle eccezioni la giornata e creo una singola
                        await RecurringToRecurring_UpdateOnlySingleInstance(request, organizer);
                        break;
                }
            }
        }

        private async Task RecurringToRecurring_UpdateOnlySingleInstance(UpdateRecurringScheduleCommand request, ApplicationUser organizer)
        {
            Guard.Against.Default(request.Input.Schedule.EndDate, nameof(request.Input.Schedule.EndDate));

            // aggiungo alle eccezioni la giornata e creo una singola
            var schedulingException = ScheduleException.Factory.Create(
                _guidGenerator.Create(),
                request.Schedule,
                request.Input.InstanceStartDate!.Value,
                request.Input.InstanceEndDate!.Value);

            await _scheduleExceptionRepository.CreateAsync(schedulingException);

            var newSingle = SingleSchedule.Factory.Create(
                _guidGenerator.Create(),
                organizer,
                request.Input.Schedule.Name,
                request.Input.Schedule.Description,
                request.Input.Schedule.ParticipantsMaxNumber,
                new Period(request.Input.InstanceStartDate.Value, request.Input.Schedule.EndDate));

            await _scheduleRepository.CreateAsync(newSingle);
        }

        private async Task RecurringToRecurring_UpdateInstanceAndFutures(UpdateRecurringScheduleCommand request, ApplicationUser organizer)
        {
            request.Schedule.UpdateDate(
                new Period(request.Schedule.Period.StartDate, request.Input.InstanceStartDate.Value),
                request.Schedule.DurationInMinutes,
                request.Schedule.RecurringCronExpressionString);

            await _scheduleRepository.UpdateAsync(request.Schedule);

            var newRecurring = RecurringSchedule.Factory.Create(
                _guidGenerator.Create(),
                organizer,
                request.Input.Schedule.Name,
                request.Input.Schedule.Description,
                request.Input.Schedule.ParticipantsMaxNumber,
                new Period(request.Input.Schedule.StartDate, request.Input.Schedule.EndDate),
                request.Input.Schedule.DurationInMinutes,
                request.Input.Schedule.RecurringCronExpression);

            await _scheduleRepository.CreateAsync(newRecurring);
        }

        private async Task RecurringToSingle(UpdateRecurringScheduleCommand request, ApplicationUser organizer)
        {
            // it was recurring, now it is single
            // devo terminare lo scheduling passato ad oggi e creare un nuovo scheduling futuro singolo con nuova start date. Le prenotazioni future vanno cancellate
            var period = new Period(request.Schedule.Period.StartDate, request.Input.InstanceStartDate.Value);
            request.Schedule.UpdateDate(period, request.Schedule.DurationInMinutes, request.Schedule.RecurringCronExpressionString);
            await _scheduleRepository.UpdateAsync(request.Schedule);

            Guard.Against.Default(request.Input.Schedule.EndDate, nameof(request.Input.Schedule.EndDate));

            var newSingle = SingleSchedule.Factory.Create(
                _guidGenerator.Create(),
                organizer,
                request.Input.Schedule.Name,
                request.Input.Schedule.Description,
                request.Input.Schedule.ParticipantsMaxNumber,
                new Period(request.Input.InstanceStartDate!.Value, request.Input.Schedule.EndDate));

            await _scheduleRepository.CreateAsync(newSingle);
        }
    }
}
