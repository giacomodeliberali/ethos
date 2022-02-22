using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cronos;
using Ethos.Application.Commands;
using Ethos.Application.Contracts.Schedule;
using Ethos.Domain.Common;
using Ethos.Query.Projections;
using Ethos.Query.Services;
using MediatR;

namespace Ethos.Application.Handlers
{
    public class
        GetAllRecurringSchedulesQueryHandler : IRequestHandler<GetAllRecurringSchedulesQuery,
            IEnumerable<RecurringScheduleDto>>
    {
        private readonly IScheduleQueryService _scheduleQueryService;
        private readonly IScheduleExceptionQueryService _scheduleExceptionQueryService;

        public GetAllRecurringSchedulesQueryHandler(
            IScheduleQueryService scheduleQueryService,
            IScheduleExceptionQueryService scheduleExceptionQueryService)
        {
            _scheduleQueryService = scheduleQueryService;
            _scheduleExceptionQueryService = scheduleExceptionQueryService;
        }

        public async Task<IEnumerable<RecurringScheduleDto>> Handle(
            GetAllRecurringSchedulesQuery request,
            CancellationToken cancellationToken)
        {
            var recurringSchedules = await _scheduleQueryService.GetAllRecurringSchedulesAsync();

            var result = new List<RecurringScheduleDto>();

            foreach (var recurringSchedule in recurringSchedules)
            {
                var nextOccurrences = await GetNextOccurrences(recurringSchedule).ToListAsync(cancellationToken);
                result.Add(new RecurringScheduleDto()
                {
                    Id = recurringSchedule.Id,
                    Name = recurringSchedule.Name,
                    Description = recurringSchedule.Description,
                    Organizer = new RecurringScheduleDto.UserDto()
                    {
                        Id = recurringSchedule.Organizer.Id,
                        Email = recurringSchedule.Organizer.Email,
                        FullName = recurringSchedule.Organizer.FullName,
                        UserName = recurringSchedule.Organizer.UserName,
                    },
                    DurationInMinutes = recurringSchedule.DurationInMinutes,
                    ParticipantsMaxNumber = recurringSchedule.ParticipantsMaxNumber,
                    RecurringCronExpression = recurringSchedule.RecurringExpression,
                    StartDate = recurringSchedule.StartDate,
                    EndDate = recurringSchedule.EndDate,
                    NextOccurrences = nextOccurrences,
                });
            }

            return result;
        }

        private async IAsyncEnumerable<DateTime> GetNextOccurrences(RecurringScheduleProjection schedule)
        {
            var now = DateTime.UtcNow;
            var period = new Period(
                schedule.StartDate >= now ? schedule.StartDate : now,
                schedule.EndDate ?? DateTime.UtcNow.AddMonths(3));

            var scheduleExceptions =
                await _scheduleExceptionQueryService.GetScheduleExceptionsAsync(schedule.Id, period);

            var nextExecutions = CronExpression.Parse(schedule.RecurringExpression).GetOccurrences(
                fromUtc: period.StartDate,
                toUtc: period.EndDate,
                fromInclusive: true,
                toInclusive: true);

            foreach (var nextExecution in nextExecutions)
            {
                var startDate = nextExecution;
                var endDate = nextExecution.Add(TimeSpan.FromMinutes(schedule.DurationInMinutes));

                var hasExceptions = scheduleExceptions.Any(e => e.StartDate <= startDate && e.EndDate >= endDate);

                if (!hasExceptions)
                {
                    yield return nextExecution;
                }
            }
        }
    }
}
