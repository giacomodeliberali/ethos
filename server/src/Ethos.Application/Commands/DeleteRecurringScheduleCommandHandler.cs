using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ethos.Application.Contracts.Schedule;
using Ethos.Domain.Common;
using Ethos.Domain.Entities;
using Ethos.Domain.Exceptions;
using Ethos.Domain.Repositories;
using Ethos.Query.Services;
using MediatR;

namespace Ethos.Application.Commands
{
    public class DeleteRecurringScheduleCommandHandler : AsyncRequestHandler<DeleteRecurringScheduleCommand>
    {
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IScheduleExceptionRepository _scheduleExceptionRepository;
        private readonly IBookingQueryService _bookingQueryService;
        private readonly IGuidGenerator _guidGenerator;

        public DeleteRecurringScheduleCommandHandler(
            IScheduleRepository scheduleRepository,
            IScheduleExceptionRepository scheduleExceptionRepository,
            IBookingQueryService bookingQueryService,
            IGuidGenerator guidGenerator)
        {
            _scheduleRepository = scheduleRepository;
            _scheduleExceptionRepository = scheduleExceptionRepository;
            _bookingQueryService = bookingQueryService;
            _guidGenerator = guidGenerator;
        }

        protected override async Task Handle(DeleteRecurringScheduleCommand request, CancellationToken cancellationToken)
        {
            var occurrences = request.Schedule.RecurringCronExpression.GetOccurrences(
                request.InstanceStartDate,
                request.InstanceEndDate,
                fromInclusive: true,
                toInclusive: true);

            if (occurrences.Count() != 1)
            {
                throw new BusinessException("Invalid instance start/end dates");
            }

            if (request.OperationType == RecurringScheduleOperationType.Future)
            {
                await DeleteFutureSchedules(request.Schedule, request.InstanceStartDate, request.InstanceEndDate);
            }
            else if (request.OperationType == RecurringScheduleOperationType.Instance)
            {
                await DeleteSingleInstanceOfRecurringSchedule(request.Schedule, request.InstanceStartDate, request.InstanceEndDate);
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

            var firstOccurrenceStartDate = schedule.RecurringCronExpression.GetNextOccurrence(schedule.StartDate, inclusive: true);
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
                        fromUtc: schedule.StartDate,
                        toUtc: instanceStartDate,
                        toInclusive: false)
                    .Last()
                    .AddMinutes(schedule.DurationInMinutes);

                schedule.UpdateDate(
                    schedule.StartDate,
                    lastPastOccurrenceEnd,
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
    }
}
