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

            var occurrences = schedule.GetOccurrences(new DateOnlyPeriod(request.InstanceStartDate, request.InstanceEndDate));

            if (occurrences.Count() != 1)
            {
                throw new BusinessException("Invalid instance start/end dates");
            }

            if (request.RecurringScheduleOperationType == RecurringScheduleOperationType.InstanceAndFuture)
            {
                await DeleteFutureSchedules(schedule, request.InstanceStartDate);
            }
            else if (request.RecurringScheduleOperationType == RecurringScheduleOperationType.Instance)
            {
                await DeleteSingleInstanceOfRecurringSchedule(schedule, request.InstanceStartDate, request.InstanceEndDate);
            }
        }

        private async Task DeleteFutureSchedules(
            RecurringSchedule schedule,
            DateTimeOffset instanceStartDate)
        {
            var futureBookings = await _bookingQueryService.GetAllBookingsInRange(
                schedule.Id,
                new DateOnlyPeriod(instanceStartDate, schedule.Period.EndDate));

            if (futureBookings.Any())
            {
                throw new BusinessException($"Non è possibile eliminare la schedulazione, sono già presenti {futureBookings.Count} prenotazioni");
            }

            var firstOccurrence = schedule.GetFirstOccurrence().StartDate;
            var isFirstOccurence = new DateOnly(firstOccurrence.Year, firstOccurrence.Month, firstOccurrence.Day) == 
                                   new DateOnly(instanceStartDate.Year, instanceStartDate.Month, instanceStartDate.Day);
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

                var lastOcc = schedule.GetOccurrences(
                    new DateOnlyPeriod(
                        schedule.Period.StartDate, 
                        new DateOnly(instanceStartDate.Year, instanceStartDate.Month, instanceStartDate.Day)))
                    .Last().EndDate.AddDays(-1);
                
                var newEndDate = new DateOnly(lastOcc.Year, lastOcc.Month, lastOcc.Day);
                
                schedule.UpdateDate(
                    new DateOnlyPeriod(schedule.Period.StartDate, newEndDate < schedule.Period.StartDate ? schedule.Period.StartDate : newEndDate),
                    schedule.DurationInMinutes,
                    schedule.RecurringCronExpressionString,
                    schedule.TimeZone);

                await _scheduleRepository.UpdateAsync(schedule);
                
                var scheduleExceptions = await _scheduleExceptionQueryService.GetScheduleExceptionsAsync(schedule.Id, new DateOnlyPeriod(schedule.Period.EndDate, DateOnly.MaxValue));
                foreach (var scheduleException in scheduleExceptions)
                {
                    await _scheduleExceptionRepository.DeleteAsync(scheduleException.Id);
                }
            }
        }

        private async Task DeleteSingleInstanceOfRecurringSchedule(
            RecurringSchedule schedule,
            DateTimeOffset instanceStartDate,
            DateTimeOffset instanceEndDate)
        {
            // add to exception table
            var existingBookings = await _bookingQueryService.GetAllBookingsInRange(
                schedule.Id,
                new DateOnlyPeriod(instanceStartDate, instanceEndDate));

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
