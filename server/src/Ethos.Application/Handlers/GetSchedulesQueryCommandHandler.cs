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
using Ethos.Query.Services;
using MediatR;

namespace Ethos.Application.Handlers
{
    public class GetSchedulesQueryCommandHandler : IRequestHandler<GetSchedulesQuery, IEnumerable<GeneratedScheduleDto>>
    {
        private readonly IScheduleQueryService _scheduleQueryService;
        private readonly IBookingQueryService _bookingQueryService;
        private readonly IScheduleExceptionQueryService _scheduleExceptionQueryService;
        private readonly ICurrentUser _currentUser;

        public GetSchedulesQueryCommandHandler(
            IScheduleQueryService scheduleQueryService,
            IBookingQueryService bookingQueryService,
            IScheduleExceptionQueryService scheduleExceptionQueryService,
            ICurrentUser currentUser)
        {
            _scheduleQueryService = scheduleQueryService;
            _bookingQueryService = bookingQueryService;
            _scheduleExceptionQueryService = scheduleExceptionQueryService;
            _currentUser = currentUser;
        }

        public async Task<IEnumerable<GeneratedScheduleDto>> Handle(GetSchedulesQuery request, CancellationToken cancellationToken)
        {
            var isAdmin = await _currentUser.IsInRole(RoleConstants.Admin);

            var result = new List<GeneratedScheduleDto>();

            var period = new Period(request.StartDate, request.EndDate);
            result.AddRange(await GetSingleSchedules(period, isAdmin));
            result.AddRange(await GenerateRecurringSchedules(period, isAdmin));

            return result.OrderBy(s => s.StartDate);
        }

        private async Task<List<GeneratedScheduleDto>> GenerateRecurringSchedules(Period period, bool isAdmin)
        {
            var recurringSchedules = await _scheduleQueryService.GetOverlappingRecurringSchedulesAsync(period);

            var result = new List<GeneratedScheduleDto>();

            foreach (var schedule in recurringSchedules)
            {
                var cronExpression = CronExpression.Parse(schedule.RecurringExpression);

                var scheduleExceptions = await _scheduleExceptionQueryService.GetScheduleExceptionsAsync(schedule.Id, period);

                var nextExecutions = cronExpression.GetOccurrences(
                    fromUtc: schedule.StartDate >= period.StartDate ? schedule.StartDate.ToUniversalTime() : period.StartDate,
                    toUtc: schedule.EndDate.HasValue && schedule.EndDate.Value <= period.EndDate ? schedule.EndDate.Value.ToUniversalTime() : period.EndDate,
                    fromInclusive: true,
                    toInclusive: true);

                foreach (var nextExecution in nextExecutions)
                {
                    var startDate = nextExecution;
                    var endDate = nextExecution.Add(TimeSpan.FromMinutes(schedule.DurationInMinutes));

                    var hasExceptions = scheduleExceptions.Any(e => e.StartDate <= startDate && e.EndDate >= endDate);

                    if (hasExceptions)
                    {
                        continue;
                    }

                    var bookings = await _bookingQueryService.GetAllBookingsInRange(schedule.Id, startDate, endDate);

                    result.Add(new GeneratedScheduleDto()
                    {
                        ScheduleId = schedule.Id,
                        Name = schedule.Name,
                        Description = schedule.Description,
                        ParticipantsMaxNumber = schedule.ParticipantsMaxNumber,
                        StartDate = startDate,
                        EndDate = endDate,
                        IsRecurring = true,
                        RecurringCronExpression = schedule.RecurringExpression,
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
            }

            return result;
        }

        private async Task<List<GeneratedScheduleDto>> GetSingleSchedules(Period period, bool isAdmin)
        {
            var singleSchedules = await _scheduleQueryService.GetOverlappingSingleSchedulesAsync(period);

            var result = new List<GeneratedScheduleDto>();

            foreach (var schedule in singleSchedules)
            {
                var startDate = schedule.StartDate;
                var endDate = schedule.StartDate.Add(TimeSpan.FromMinutes(schedule.DurationInMinutes));
                var bookings = await _bookingQueryService.GetAllBookingsInRange(schedule.Id, startDate, endDate);

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
