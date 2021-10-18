using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Ethos.Application.Commands;
using Ethos.Application.Contracts.Schedule;
using Ethos.Domain.Common;
using Ethos.Domain.Entities;
using Ethos.Domain.Exceptions;
using Ethos.Domain.Extensions;
using Ethos.Domain.Repositories;
using Ethos.Shared;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Ethos.Application.Handlers
{
    public class UpdateScheduleCommandHandler : AsyncRequestHandler<UpdateScheduleCommand>
    {
        private readonly IScheduleRepository _scheduleRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IGuidGenerator _guidGenerator;
        private readonly IScheduleExceptionRepository _scheduleExceptionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateScheduleCommandHandler(
            IScheduleRepository scheduleRepository,
            UserManager<ApplicationUser> userManager,
            IGuidGenerator guidGenerator,
            IScheduleExceptionRepository scheduleExceptionRepository,
            IUnitOfWork unitOfWork)
        {
            _scheduleRepository = scheduleRepository;
            _userManager = userManager;
            _guidGenerator = guidGenerator;
            _scheduleExceptionRepository = scheduleExceptionRepository;
            _unitOfWork = unitOfWork;
        }

        protected override async Task Handle(UpdateScheduleCommand request, CancellationToken cancellationToken)
        {
            var schedule = await _scheduleRepository.GetByIdAsync(request.Id);

            if (schedule is RecurringSchedule recurringSchedule)
            {
                await UpdateSchedule(recurringSchedule, request);
            }
            else if (schedule is SingleSchedule singleSchedule)
            {
                await UpdateSchedule(singleSchedule, request);
            }

            await _unitOfWork.SaveChangesAsync();
        }

        /// <summary>
        /// I'm updating an instance of a specific recurring schedule.
        /// I can choose to update only this specific instance of this and all the future ones.
        ///
        /// The schedule could remain recurring or it could be converted into a single (thus deleting the future occurrences).
        /// </summary>
        private async Task UpdateSchedule(RecurringSchedule schedule, UpdateScheduleCommand request)
        {
            var occurrences = schedule.RecurringCronExpression.GetOccurrences(
                request.InstanceStartDate,
                request.InstanceEndDate,
                fromInclusive: true,
                toInclusive: true).ToList();

            if (occurrences.Count != 1 || occurrences.Single() < schedule.Period.StartDate || occurrences.Single() > schedule.Period.EndDate)
            {
                throw new BusinessException("Invalid instance start/end dates");
            }

            var organizer = await _userManager.FindByIdAsync(request.UpdatedSchedule.OrganizerId);

            if (organizer == null || !await _userManager.IsInRoleAsync(organizer, RoleConstants.Admin))
            {
                throw new BusinessException("Invalid organizer id");
            }

            if (string.IsNullOrEmpty(request.UpdatedSchedule.RecurringCronExpression))
            {
                // it was recurring, now it is single
                // update the recurring schedule's end date and create a single future schedule
                // future bookings will be canceled.
                await RecurringToSingle(request, organizer, schedule);
            }
            else
            {
                Guard.Against.Null(request.RecurringScheduleOperationType, nameof(request.RecurringScheduleOperationType));

                switch (request.RecurringScheduleOperationType)
                {
                    case RecurringScheduleOperationType.Future:
                        // update the recurring schedule's end date and create a new recurring schedule with the new info. Future bookings will be canceled.
                        await RecurringToRecurring_UpdateInstanceAndFutures(request, organizer, schedule);
                        break;
                    case RecurringScheduleOperationType.Instance:
                        // do not update the recurring schedule, just create an exception for that day
                        await RecurringToRecurring_UpdateOnlySingleInstance(request, organizer, schedule);
                        break;
                }
            }
        }

        private async Task RecurringToRecurring_UpdateOnlySingleInstance(UpdateScheduleCommand request, ApplicationUser organizer, RecurringSchedule schedule)
        {
            // aggiungo alle eccezioni la giornata e creo una singola
            var schedulingException = ScheduleException.Factory.Create(
                _guidGenerator.Create(),
                schedule,
                request.InstanceStartDate,
                request.InstanceEndDate);

            await _scheduleExceptionRepository.CreateAsync(schedulingException);

            var newSingle = SingleSchedule.Factory.Create(
                _guidGenerator.Create(),
                organizer,
                request.UpdatedSchedule.Name,
                request.UpdatedSchedule.Description,
                request.UpdatedSchedule.ParticipantsMaxNumber,
                new Period(request.InstanceStartDate, request.UpdatedSchedule.EndDate));

            await _scheduleRepository.CreateAsync(newSingle);
        }

        private async Task RecurringToRecurring_UpdateInstanceAndFutures(UpdateScheduleCommand request, ApplicationUser organizer, RecurringSchedule schedule)
        {
            schedule.UpdateDate(
                new Period(schedule.Period.StartDate, request.InstanceStartDate),
                schedule.DurationInMinutes,
                schedule.RecurringCronExpressionString);

            await _scheduleRepository.UpdateAsync(schedule);

            var newRecurring = RecurringSchedule.Factory.Create(
                _guidGenerator.Create(),
                organizer,
                request.UpdatedSchedule.Name,
                request.UpdatedSchedule.Description,
                request.UpdatedSchedule.ParticipantsMaxNumber,
                new Period(request.UpdatedSchedule.StartDate, request.UpdatedSchedule.EndDate),
                request.UpdatedSchedule.DurationInMinutes,
                request.UpdatedSchedule.RecurringCronExpression);

            await _scheduleRepository.CreateAsync(newRecurring);
        }

        private async Task RecurringToSingle(UpdateScheduleCommand request, ApplicationUser organizer, RecurringSchedule schedule)
        {
            // it was recurring, now it is single
            // devo terminare lo scheduling passato ad oggi e creare un nuovo scheduling futuro singolo con nuova start date. Le prenotazioni future vanno cancellate
            var period = new Period(schedule.Period.StartDate, request.InstanceStartDate);
            schedule.UpdateDate(period, schedule.DurationInMinutes, schedule.RecurringCronExpressionString);
            await _scheduleRepository.UpdateAsync(schedule);

            var newSingle = SingleSchedule.Factory.Create(
                _guidGenerator.Create(),
                organizer,
                request.UpdatedSchedule.Name,
                request.UpdatedSchedule.Description,
                request.UpdatedSchedule.ParticipantsMaxNumber,
                new Period(request.InstanceStartDate, request.UpdatedSchedule.EndDate));

            await _scheduleRepository.CreateAsync(newSingle);
        }

        /// <summary>
        /// Updates a single schedule: it can remain a single schedule or become a recurring schedule if a CRON is provided.
        /// </summary>
        private async Task UpdateSchedule(SingleSchedule schedule, UpdateScheduleCommand request)
        {
            var organizer = await _userManager.FindByIdAsync(request.UpdatedSchedule.OrganizerId.ToString());

            if (organizer == null || !await _userManager.IsInRoleAsync(organizer, RoleConstants.Admin))
            {
                throw new BusinessException("Invalid organizer id");
            }

            if (!string.IsNullOrEmpty(request.UpdatedSchedule.RecurringCronExpression))
            {
                // now it is recurring, create a new one with the same id
                await _scheduleRepository.DeleteAsync(schedule);

                var newRecurring = RecurringSchedule.Factory.Create(
                    schedule.Id,
                    organizer,
                    request.UpdatedSchedule.Name,
                    request.UpdatedSchedule.Description,
                    request.UpdatedSchedule.ParticipantsMaxNumber,
                    new Period(request.UpdatedSchedule.StartDate, request.UpdatedSchedule.EndDate),
                    request.UpdatedSchedule.DurationInMinutes,
                    request.UpdatedSchedule.RecurringCronExpression);

                await _scheduleRepository.CreateAsync(newRecurring);
            }
            else
            {
                // just update
                schedule!.UpdateOrganizer(organizer);
                schedule.UpdateNameAndDescription(request.UpdatedSchedule.Name, request.UpdatedSchedule.Description);
                schedule.UpdateParticipantsMaxNumber(request.UpdatedSchedule.ParticipantsMaxNumber);
                schedule.UpdatePeriod(new Period(request.UpdatedSchedule.StartDate, request.UpdatedSchedule.EndDate));

                await _scheduleRepository.UpdateAsync(schedule);
            }
        }
    }
}
