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
    public class ScheduleApplicationService : BaseApplicationService, IScheduleApplicationService
    {
        private readonly IScheduleRepository _scheduleRepository;
        private readonly ICurrentUser _currentUser;
        private readonly IScheduleQueryService _scheduleQueryService;

        public ScheduleApplicationService(
            IUnitOfWork unitOfWork,
            IScheduleRepository scheduleRepository,
            ICurrentUser currentUser,
            IScheduleQueryService scheduleQueryService)
        : base(unitOfWork)
        {
            _scheduleRepository = scheduleRepository;
            _currentUser = currentUser;
            _scheduleQueryService = scheduleQueryService;
        }

        /// <inheritdoc />
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

            var scheduleId = await _scheduleRepository.CreateAsync(schedule);

            await UnitOfWork.SaveChangesAsync();

            return scheduleId;
        }

        /// <inheritdoc />
        public async Task UpdateAsync(UpdateScheduleRequestDto input)
        {
            var schedule = await _scheduleRepository.GetByIdAsync(input.Id);

            schedule
                .UpdateNameAndDescription(input.Name, input.Description)
                .UpdateDateTime(
                    input.StartDate!.Value,
                    input.EndDate,
                    input.Duration,
                    input.RecurringCronExpression);

            await _scheduleRepository.UpdateAsync(schedule);

            await UnitOfWork.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task DeleteAsync(Guid id)
        {
            var schedule = await _scheduleRepository.GetByIdAsync(id);
            await _scheduleRepository.DeleteAsync(schedule);
            await UnitOfWork.SaveChangesAsync();
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
