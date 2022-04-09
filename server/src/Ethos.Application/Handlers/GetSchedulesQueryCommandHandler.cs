using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cronos;
using Ethos.Application.Contracts.Schedule;
using Ethos.Application.Identity;
using Ethos.Application.Queries;
using Ethos.Common;
using Ethos.Domain.Common;
using Ethos.Domain.Entities;
using Ethos.Domain.Repositories;
using Ethos.Query.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ethos.Application.Handlers
{
    public class GetSchedulesQueryCommandHandler : IRequestHandler<GetSchedulesQuery, IEnumerable<GeneratedScheduleDto>>
    {
        private readonly IScheduleQueryService _scheduleQueryService;
        private readonly IBookingQueryService _bookingQueryService;
        private readonly IScheduleExceptionQueryService _scheduleExceptionQueryService;
        private readonly ICurrentUser _currentUser;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly ILogger<GetSchedulesQueryCommandHandler> _logger;

        public GetSchedulesQueryCommandHandler(
            IScheduleQueryService scheduleQueryService,
            IBookingQueryService bookingQueryService,
            IScheduleExceptionQueryService scheduleExceptionQueryService,
            ICurrentUser currentUser,
            IScheduleRepository scheduleRepository,
            ILogger<GetSchedulesQueryCommandHandler> logger)
        {
            _scheduleQueryService = scheduleQueryService;
            _bookingQueryService = bookingQueryService;
            _scheduleExceptionQueryService = scheduleExceptionQueryService;
            _currentUser = currentUser;
            _scheduleRepository = scheduleRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<GeneratedScheduleDto>> Handle(GetSchedulesQuery request, CancellationToken cancellationToken)
        {
            var isAdmin = await _currentUser.IsInRole(RoleConstants.Admin);

            var result = new List<GeneratedScheduleDto>();

            var period = new DateOnlyPeriod(request.StartDate, request.EndDate);
            result.AddRange(await GetSingleSchedules(period, isAdmin));
            result.AddRange(await GenerateRecurringSchedules(period, isAdmin));

            return result.OrderBy(s => s.StartDate);
        }

        private async Task<List<GeneratedScheduleDto>> GenerateRecurringSchedules(DateOnlyPeriod period, bool isAdmin)
        {
            var recurringSchedulesProjections = await _scheduleQueryService.GetOverlappingRecurringSchedulesAsync(period);

            var recurringSchedules = 
                (await _scheduleRepository.GetByIdAsync(recurringSchedulesProjections.Select(p => p.Id)))
                .Select(s => (RecurringSchedule)s);
            
            var result = new List<GeneratedScheduleDto>();
            
            _logger.LogDebug("[GenerateRecurringSchedules] For period {StartDate} - {EndDate}", period.StartDate, period.EndDate);
            
            foreach (var recurringSchedule in recurringSchedules)
            {
                _logger.LogDebug("[GetScheduleExceptionsAsync] For Schedule {Id} with timeZone {TimeZone}", recurringSchedule.Id, recurringSchedule.TimeZone.Id);
                
                var scheduleExceptions = await _scheduleExceptionQueryService.GetScheduleExceptionsAsync(recurringSchedule.Id, period);
                
                _logger.LogDebug("[GetScheduleExceptionsAsync] Got {Count} exceptions", scheduleExceptions.Count);
                
                foreach (var scheduleExtensionProjection in scheduleExceptions)
                {
                    _logger.LogDebug("[ScheduleException] From {StartDate} to {EndDate}", scheduleExtensionProjection.StartDate, scheduleExtensionProjection.EndDate);
                }
                
                var nextExecutions = recurringSchedule.GetOccurrences(period, recurringSchedule.TimeZone);

                foreach (var (nextStartDate, nextEndDate) in nextExecutions)
                {
                    _logger.LogDebug("NextExecution = {NextStart} to {NextEnd}",  nextStartDate, nextEndDate);

                    var hasExceptions = scheduleExceptions.Any(e => e.StartDate <= nextStartDate && e.EndDate >= nextEndDate);

                    _logger.LogDebug("HasExceptions {Value}", hasExceptions);
                    
                    if (hasExceptions)
                    {
                        continue;
                    }

                    var bookings = await _bookingQueryService.GetAllBookingsInRange(
                        recurringSchedule.Id,
                        new DateOnlyPeriod(nextStartDate, nextEndDate));

                    result.Add(new GeneratedScheduleDto()
                    {
                        ScheduleId = recurringSchedule.Id,
                        Name = recurringSchedule.Name,
                        Description = recurringSchedule.Description,
                        ParticipantsMaxNumber = recurringSchedule.ParticipantsMaxNumber,
                        StartDate = nextStartDate.DateTime,
                        EndDate = nextEndDate.DateTime,
                        IsRecurring = true,
                        RecurringCronExpression = recurringSchedule.RecurringCronExpressionString,
                        Organizer = new GeneratedScheduleDto.UserDto()
                        {
                            Id = recurringSchedule.Organizer.Id,
                            FullName = recurringSchedule.Organizer.FullName,
                            Email = recurringSchedule.Organizer.Email,
                            UserName = recurringSchedule.Organizer.UserName,
                        },
                        Bookings = bookings.Select(b => new GeneratedScheduleDto.BookingDto()
                        {
                            Id = b.Id,
                            User = isAdmin || b.UserId == _currentUser.UserId()
                                ? new GeneratedScheduleDto.UserDto()
                                {
                                    Id = b.UserId,
                                    FullName = b.UserFullName,
                                    Email = b.UserEmail,
                                    UserName = b.UserName,
                                }
                                : null,
                        }),
                    });
                }
            }

            return result;
        }

        private async Task<List<GeneratedScheduleDto>> GetSingleSchedules(DateOnlyPeriod period, bool isAdmin)
        {
            var singleSchedules = await _scheduleQueryService.GetOverlappingSingleSchedulesAsync(period);

            var result = new List<GeneratedScheduleDto>();

            foreach (var schedule in singleSchedules)
            {
                var startDate = schedule.StartDate;
                var endDate = schedule.StartDate.Add(TimeSpan.FromMinutes(schedule.DurationInMinutes));
                var bookings = await _bookingQueryService.GetAllBookingsInRange(schedule.Id, new DateOnlyPeriod(startDate,endDate));

                result.Add(new GeneratedScheduleDto()
                {
                    ScheduleId = schedule.Id,
                    Name = schedule.Name,
                    Description = schedule.Description,
                    ParticipantsMaxNumber = schedule.ParticipantsMaxNumber,
                    StartDate = startDate,
                    EndDate = endDate,
                    IsRecurring = false,
                    Organizer = new GeneratedScheduleDto.UserDto()
                    {
                        Id = schedule.Organizer.Id,
                        FullName = schedule.Organizer.FullName,
                        Email = schedule.Organizer.Email,
                        UserName = schedule.Organizer.UserName,
                    },
                    Bookings = bookings.Select(b => new GeneratedScheduleDto.BookingDto()
                    {
                        Id = b.Id,
                        User = isAdmin || b.UserId == _currentUser.UserId()
                            ? new GeneratedScheduleDto.UserDto()
                            {
                                Id = b.UserId,
                                FullName = b.UserFullName,
                                Email = b.UserEmail,
                                UserName = b.UserName,
                            }
                            : null,
                    }),
                });
            }

            return result;
        }
    }
}
