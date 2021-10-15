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
using Ethos.Domain.Repositories;
using Ethos.Query.Services;
using MediatR;

namespace Ethos.Application.Handlers
{
    public class DeleteScheduleCommandHandler : AsyncRequestHandler<DeleteScheduleCommand>
    {
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBookingQueryService _bookingQueryService;
        private readonly IScheduleExceptionRepository _scheduleExceptionRepository;
        private readonly IGuidGenerator _guidGenerator;

        public DeleteScheduleCommandHandler(
            IScheduleRepository scheduleRepository,
            IUnitOfWork unitOfWork,
            IBookingQueryService bookingQueryService,
            IScheduleExceptionRepository scheduleExceptionRepository,
            IGuidGenerator guidGenerator)
        {
            _scheduleRepository = scheduleRepository;
            _unitOfWork = unitOfWork;
            _bookingQueryService = bookingQueryService;
            _scheduleExceptionRepository = scheduleExceptionRepository;
            _guidGenerator = guidGenerator;
        }

        protected override async Task Handle(DeleteScheduleCommand request, CancellationToken cancellationToken)
        {
            var schedule = await _scheduleRepository.GetByIdAsync(request.Id);

            if (schedule is RecurringSchedule recurringSchedule)
            {
                await DeleteSchedule(recurringSchedule, request);
            }
            else if (schedule is SingleSchedule singleSchedule)
            {
                await DeleteSchedule(singleSchedule, request);
            }

            await _unitOfWork.SaveChangesAsync();
        }

        private async Task DeleteSchedule(RecurringSchedule schedule, DeleteScheduleCommand request)
        {
            Guard.Against.Null(request.RecurringScheduleOperationType, nameof(request.RecurringScheduleOperationType));

            var occurrences = schedule.RecurringCronExpression.GetOccurrences(
                request.InstanceStartDate,
                request.InstanceEndDate,
                fromInclusive: true,
                toInclusive: true);

            if (occurrences.Count() != 1)
            {
                throw new BusinessException("Invalid instance start/end dates");
            }

            if (request.RecurringScheduleOperationType == RecurringScheduleOperationType.Future)
            {
                await DeleteFutureSchedules(schedule, request.InstanceStartDate, request.InstanceEndDate);
            }
            else if (request.RecurringScheduleOperationType == RecurringScheduleOperationType.Instance)
            {
                await DeleteSingleInstanceOfRecurringSchedule(schedule, request.InstanceStartDate, request.InstanceEndDate);
            }
        }

        private async Task DeleteFutureSchedules(
            RecurringSchedule schedule,
            DateTime instanceStartDate,
            DateTime instanceEndDate)
        {
            var futureBookings = await _bookingQueryService.GetAllBookingsInRange(
                schedule.Id,
                startDate: instanceStartDate,
                endDate: DateTime.MaxValue);

            if (futureBookings.Any())
            {
                throw new BusinessException($"Non è possibile eliminare la schedulazione, sono già presenti {futureBookings.Count} prenotazioni");
            }

            var firstOccurrenceStartDate = schedule.RecurringCronExpression.GetNextOccurrence(schedule.Period.StartDate, inclusive: true);
            var isFirstOccurence = firstOccurrenceStartDate == instanceStartDate;
            if (isFirstOccurence)
            {
                // I am deleting the first occurrence, just delete everything
                await _scheduleRepository.DeleteAsync(schedule);
            }
            else
            {
                // I am editing an occurrence in the middle. Make the past end at last occurence (but do not delete the past!)
                var lastPastOccurrenceEnd = schedule.RecurringCronExpression.GetOccurrences(
                        fromUtc: schedule.Period.StartDate,
                        toUtc: instanceStartDate,
                        toInclusive: false)
                    .Last()
                    .AddMinutes(schedule.DurationInMinutes);

                schedule.UpdateDate(
                    new Period(schedule.Period.StartDate, lastPastOccurrenceEnd),
                    schedule.DurationInMinutes,
                    schedule.RecurringCronExpressionString);

                await _scheduleRepository.UpdateAsync(schedule);
            }
        }

        private async Task DeleteSingleInstanceOfRecurringSchedule(
            RecurringSchedule schedule,
            DateTime instanceStartDate,
            DateTime instanceEndDate)
        {
            // add to exception table
            var existingBookings = await _bookingQueryService.GetAllBookingsInRange(
                schedule.Id,
                instanceStartDate,
                instanceEndDate);

            if (existingBookings.Any())
            {
                throw new BusinessException(
                    $"Non è possibile eliminare la schedulazione, sono presenti {existingBookings.Count} prenotazioni");
            }

            var scheduleException = ScheduleException.Factory.Create(
                _guidGenerator.Create(),
                schedule,
                instanceStartDate,
                instanceEndDate);

            await _scheduleExceptionRepository.CreateAsync(scheduleException);
        }

        private async Task DeleteSchedule(SingleSchedule schedule, DeleteScheduleCommand request)
        {
            var existingBookings = await _bookingQueryService.GetAllBookingsInRange(
                schedule.Id,
                schedule.Period.StartDate,
                schedule.Period.EndDate);

            if (existingBookings.Any())
            {
                throw new BusinessException($"Non è possibile eliminare la schedulazione, sono presenti {existingBookings.Count} prenotazioni");
            }

            await _scheduleRepository.DeleteAsync(schedule);
        }
    }
}
