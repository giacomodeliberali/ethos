using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cronos;
using Ethos.Application.Contracts.Schedule;
using Ethos.Application.Identity;
using Ethos.Application.Queries;
using Ethos.Domain.Common;
using Ethos.Query.Services;
using Ethos.Shared;
using MediatR;

namespace Ethos.Application.Handlers
{
    public class GetSchedulesQueryCommandHandler : IRequestHandler<GetSchedulesQueryCommand, IEnumerable<GeneratedScheduleDto>>
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

        public async Task<IEnumerable<GeneratedScheduleDto>> Handle(GetSchedulesQueryCommand request, CancellationToken cancellationToken)
        {
            var startDateStartOfDay = request.StartDate.Date.ToUniversalTime();
            var endDateEndOfDay = request.EndDate.Date.AddDays(1).AddTicks(-1).ToUniversalTime();

            var isAdmin = await _currentUser.IsInRole(RoleConstants.Admin);

            var result = new List<GeneratedScheduleDto>();

            result.AddRange(await GetSingleSchedules(new Period(startDateStartOfDay, endDateEndOfDay), isAdmin));
            result.AddRange(await GenerateRecurringSchedules(startDateStartOfDay, endDateEndOfDay, isAdmin));

            return result.OrderBy(s => s.StartDate);
        }

        private async Task<List<GeneratedScheduleDto>> GenerateRecurringSchedules(
            DateTime startDateStartOfDay,
            DateTime endDateEndOfDay,
            bool isAdmin)
        {
            var recurringSchedules = await _scheduleQueryService.GetOverlappingRecurringSchedulesAsync(new Period(startDateStartOfDay, endDateEndOfDay));

            var result = new List<GeneratedScheduleDto>();

            foreach (var schedule in recurringSchedules)
            {
                var cronExpression = CronExpression.Parse(schedule.RecurringExpression);

                var scheduleExceptions = await _scheduleExceptionQueryService.GetScheduleExceptionsAsync(schedule.Id, startDateStartOfDay, endDateEndOfDay);

                var nextExecutions = cronExpression.GetOccurrences(
                    fromUtc: schedule.StartDate >= startDateStartOfDay ? schedule.StartDate.ToUniversalTime() : startDateStartOfDay,
                    toUtc: schedule.EndDate.HasValue && schedule.EndDate.Value <= endDateEndOfDay ? schedule.EndDate.Value.ToUniversalTime() : endDateEndOfDay,
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
                            User = isAdmin
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
                        User = isAdmin
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
