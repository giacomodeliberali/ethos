using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Cronos;
using Ethos.Application.Contracts.Schedule;
using Ethos.Application.Identity;
using Ethos.Domain.Entities;
using Ethos.Domain.Repositories;
using Ethos.Query;

namespace Ethos.Application.Services
{
    public class ScheduleApplicationService : IScheduleApplicationService
    {
        private readonly IScheduleRepository _scheduleRepository;
        private readonly ICurrentUser _currentUser;
        private readonly IScheduleQueryService _scheduleQueryService;

        public ScheduleApplicationService(
            IScheduleRepository scheduleRepository,
            ICurrentUser currentUser,
            IScheduleQueryService scheduleQueryService)
        {
            _scheduleRepository = scheduleRepository;
            _currentUser = currentUser;
            _scheduleQueryService = scheduleQueryService;
        }

        public async Task<Guid> CreateAsync(CreateScheduleRequestDto input)
        {
            var currentUser = await _currentUser.GetCurrentUser();

            Schedule schedule;

            if (string.IsNullOrEmpty(input.RecurringCronExpression))
            {
                Guard.Against.Null(input.StartDate, nameof(input.StartDate));
                Guard.Against.Null(input.EndDate, nameof(input.EndDate));

                schedule = Schedule.Factory.CreateNonRecurring(
                    currentUser,
                    input.Name,
                    input.Description,
                    input.StartDate.Value,
                    input.EndDate.Value);
            }
            else
            {
                Guard.Against.Null(input.StartDate, nameof(input.StartDate));
                Guard.Against.Null(input.Duration, nameof(input.Duration));
                Guard.Against.Null(input.RecurringCronExpression, nameof(input.RecurringCronExpression));

                schedule = Schedule.Factory.CreateRecurring(
                    currentUser,
                    input.Name,
                    input.Description,
                    input.StartDate.Value,
                    input.EndDate,
                    input.Duration.Value,
                    input.RecurringCronExpression);
            }

            return await _scheduleRepository.CreateAsync(schedule);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<GeneratedScheduleDto>> GetSchedules(DateTime from, DateTime to)
        {
            var schedules = await _scheduleQueryService.GetInRangeAsync(from, to);

            var result = new List<GeneratedScheduleDto>();

            foreach (var schedule in schedules)
            {
                if (string.IsNullOrEmpty(schedule.RecurringExpression))
                {
                    result.Add(new GeneratedScheduleDto()
                    {
                        Name = schedule.Name,
                        Description = schedule.Description,
                        StartDate = schedule.StartDate,
                        EndDate = schedule.StartDate.Add(schedule.Duration),
                    });
                    continue;
                }

                var cronExpression = CronExpression.Parse(schedule.RecurringExpression);

                var nextExecutions = cronExpression.GetOccurrences(
                    fromUtc: from,
                    toUtc: to,
                    zone: TimeZoneInfo.Local,
                    fromInclusive: true,
                    toInclusive: true);

                foreach (var nextExecution in nextExecutions)
                {
                    result.Add(new GeneratedScheduleDto()
                    {
                        Name = schedule.Name,
                        Description = schedule.Description,
                        StartDate = nextExecution,
                        EndDate = schedule.StartDate.Add(schedule.Duration),
                    });
                }
            }

            return result;
        }
    }
}
