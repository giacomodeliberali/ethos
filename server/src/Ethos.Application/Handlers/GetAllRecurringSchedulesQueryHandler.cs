using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cronos;
using Ethos.Application.Commands;
using Ethos.Application.Contracts.Schedule;
using Ethos.Query.Services;
using MediatR;

namespace Ethos.Application.Handlers
{
    public class GetAllRecurringSchedulesQueryHandler : IRequestHandler<GetAllRecurringSchedulesQuery, IEnumerable<RecurringScheduleDto>>
    {
        private readonly IScheduleQueryService _scheduleQueryService;

        public GetAllRecurringSchedulesQueryHandler(IScheduleQueryService scheduleQueryService)
        {
            _scheduleQueryService = scheduleQueryService;
        }

        public async Task<IEnumerable<RecurringScheduleDto>> Handle(GetAllRecurringSchedulesQuery request, CancellationToken cancellationToken)
        {
            var recurringSchedules = await _scheduleQueryService.GetAllRecurringSchedulesAsync();

            return recurringSchedules.Select(s => new RecurringScheduleDto()
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
                Organizer = new RecurringScheduleDto.UserDto()
                {
                    Id = s.Organizer.Id,
                    Email = s.Organizer.Email,
                    FullName = s.Organizer.FullName,
                    UserName = s.Organizer.UserName,
                },
                DurationInMinutes = s.DurationInMinutes,
                ParticipantsMaxNumber = s.ParticipantsMaxNumber,
                RecurringCronExpression = s.RecurringExpression,
                StartDate = s.StartDate,
                EndDate = s.EndDate,
                NextOccurrences = GetNextOccurrences(s.RecurringExpression, s.EndDate),
            });
        }

        private IEnumerable<DateTime> GetNextOccurrences(string recurringExpression, DateTime? endDate)
        {
            var endDateOccurrence = endDate ?? DateTime.UtcNow.AddMonths(3);
            var cronExpression = CronExpression.Parse(recurringExpression);
            return cronExpression.GetOccurrences(DateTime.UtcNow, endDateOccurrence);
        }
    }
}
