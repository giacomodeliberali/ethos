using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cronos;
using Ethos.Application.Contracts.Schedule;
using Ethos.Application.Queries;
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
                });
            }

            return result;
        }
    }
}
