using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Ethos.Application.Commands.Schedules.Recurring;
using Ethos.Application.Contracts;
using Ethos.Application.Contracts.Schedule;
using Ethos.Application.Exceptions;
using Ethos.Domain.Common;
using Ethos.Domain.Entities;
using Ethos.Domain.Exceptions;
using Ethos.Domain.Repositories;
using Ethos.Query.Services;
using MediatR;

namespace Ethos.Application.Handlers.Schedules.Recurring
{
    public class DeleteRecurringScheduleCommandHandler : IRequestHandler<DeleteRecurringScheduleCommand, DeleteScheduleReplyDto>
    {
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBookingQueryService _bookingQueryService;
        private readonly IScheduleExceptionRepository _scheduleExceptionRepository;
        private readonly IScheduleExceptionQueryService _scheduleExceptionQueryService;
        private readonly IGuidGenerator _guidGenerator;

        public DeleteRecurringScheduleCommandHandler(
            IScheduleRepository scheduleRepository,
            IUnitOfWork unitOfWork,
            IBookingQueryService bookingQueryService,
            IScheduleExceptionRepository scheduleExceptionRepository,
            IScheduleExceptionQueryService scheduleExceptionQueryService,
            IGuidGenerator guidGenerator)
        {
            _scheduleRepository = scheduleRepository;
            _unitOfWork = unitOfWork;
            _bookingQueryService = bookingQueryService;
            _scheduleExceptionRepository = scheduleExceptionRepository;
            _scheduleExceptionQueryService = scheduleExceptionQueryService;
            _guidGenerator = guidGenerator;
        }

        public async Task<DeleteScheduleReplyDto> Handle(DeleteRecurringScheduleCommand request, CancellationToken cancellationToken)
        {
            var result = new DeleteScheduleReplyDto
            {
                AffectedUsers = new List<DeleteScheduleReplyDto.UserDto>(),
            };

            var schedule = await _scheduleRepository.GetByIdAsync(request.Id);

            if (schedule is SingleSchedule)
            {
                throw new BusinessException("Can not delete single schedule");
            }
            
            await DeleteSchedule((RecurringSchedule)schedule, request);

            await _unitOfWork.SaveChangesAsync();

            return result;
        }

        private async Task DeleteSchedule(RecurringSchedule schedule, DeleteRecurringScheduleCommand request)
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

            if (request.RecurringScheduleOperationType == RecurringScheduleOperationType.InstanceAndFuture)
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
                endDate: schedule.Period.EndDate);

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
                var scheduleExceptions = await _scheduleExceptionQueryService.GetScheduleExceptionsAsync(schedule.Id);
                foreach (var scheduleException in scheduleExceptions)
                {
                    await _scheduleExceptionRepository.DeleteAsync(scheduleException.Id);
                }
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
                
                var scheduleExceptions = await _scheduleExceptionQueryService.GetScheduleExceptionsAsync(schedule.Id, new Period(schedule.Period.EndDate, DateTime.MaxValue.ToUniversalTime()));
                foreach (var scheduleException in scheduleExceptions)
                {
                    await _scheduleExceptionRepository.DeleteAsync(scheduleException.Id);
                }
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
                throw new CanNotDeleteScheduleWithExistingBookingsException(existingBookings.Count);
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
