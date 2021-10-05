using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Cronos;
using Ethos.Application.Contracts.Schedule;
using Ethos.Application.Identity;
using Ethos.Domain.Repositories;
using Ethos.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Ethos.Application.Schedule
{
    public class ScheduleApplicationService : IScheduleApplicationService
    {
        private readonly IScheduleRepository _scheduleRepository;
        private readonly ICurrentUser _currentUser;
        private readonly ApplicationDbContext _applicationDbContext;

        public ScheduleApplicationService(
            IScheduleRepository scheduleRepository,
            ICurrentUser currentUser,
            ApplicationDbContext applicationDbContext)
        {
            _scheduleRepository = scheduleRepository;
            _currentUser = currentUser;
            _applicationDbContext = applicationDbContext;
        }

        public async Task<Guid> CreateAsync(CreateScheduleRequestDto input)
        {
            var currentUser = await _currentUser.GetCurrentUser();

            Domain.Schedule.Schedule schedule;

            if (string.IsNullOrEmpty(input.RecurringCronExpression))
            {
                Guard.Against.Null(input.StartDate, nameof(input.StartDate));
                Guard.Against.Null(input.EndDate, nameof(input.EndDate));

                schedule = Domain.Schedule.Schedule.Factory.CreateNonRecurring(
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

                schedule = Domain.Schedule.Schedule.Factory.CreateRecurring(
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
            // TODO move query to separate project
            var schedules = await _applicationDbContext.Schedules
                .AsQueryable()
                .Where(s => s.StartDate >= from && s.EndDate <= to)
                .ToListAsync();

            var result = new List<GeneratedScheduleDto>();

            foreach (var schedule in schedules)
            {
                if (string.IsNullOrEmpty(schedule.RecurringExpression))
                {
                    result.Add(new GeneratedScheduleDto()
                    {
                        StartDate = schedule.StartDate,
                        EndDate = schedule.EndDate!.Value,
                        Name = "Single schedule",
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
                        StartDate = nextExecution,
                        EndDate = nextExecution.AddMinutes(60), // TODO add Duration property in schedule
                        Name = "Repeated schedule",
                    });
                }
            }

            return result;
        }
    }
}
