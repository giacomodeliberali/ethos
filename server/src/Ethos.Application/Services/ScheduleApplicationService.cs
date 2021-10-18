using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ethos.Application.Commands;
using Ethos.Application.Contracts.Schedule;
using Ethos.Application.Queries;
using Ethos.Domain.Repositories;
using MediatR;

namespace Ethos.Application.Services
{
    /// <summary>
    /// Contains the use cases for the web UI.
    /// </summary>
    public class ScheduleApplicationService : BaseApplicationService, IScheduleApplicationService
    {
        public ScheduleApplicationService(
            IMediator mediator,
            IUnitOfWork unitOfWork)
            : base(mediator, unitOfWork)
        {
        }

        /// <inheritdoc />
        public async Task<CreateScheduleReplyDto> CreateAsync(CreateScheduleRequestDto input)
        {
            var scheduleId = await Mediator.Send(new CreateScheduleCommand(
                input.Name,
                input.Description,
                input.StartDate,
                input.EndDate,
                input.DurationInMinutes,
                input.RecurringCronExpression,
                input.ParticipantsMaxNumber,
                input.OrganizerId));

            return new CreateScheduleReplyDto()
            {
                Id = scheduleId,
            };
        }

        /// <inheritdoc />
        public async Task UpdateAsync(UpdateScheduleRequestDto input)
        {
            await Mediator.Send(new UpdateScheduleCommand()
            {
                Id = input.Id,
                InstanceStartDate = input.InstanceStartDate,
                InstanceEndDate = input.InstanceEndDate,
                RecurringScheduleOperationType = input.RecurringScheduleOperationType,
                UpdatedSchedule = new UpdateScheduleCommand.Schedule()
                {
                    Name = input.Schedule.Name,
                    Description = input.Schedule.Description,
                    StartDate = input.Schedule.StartDate,
                    EndDate = input.Schedule.EndDate,
                    OrganizerId = input.Schedule.OrganizerId,
                    DurationInMinutes = input.Schedule.DurationInMinutes,
                    ParticipantsMaxNumber = input.Schedule.ParticipantsMaxNumber,
                    RecurringCronExpression = input.Schedule.RecurringCronExpression,
                },
            });
        }

        /// <inheritdoc />
        public async Task DeleteAsync(DeleteScheduleRequestDto input)
        {
            await Mediator.Send(new DeleteScheduleCommand
            {
                Id = input.Id,
                InstanceStartDate = input.InstanceStartDate,
                InstanceEndDate = input.InstanceEndDate,
                RecurringScheduleOperationType = input.RecurringScheduleOperationType,
            });
        }

        /// <inheritdoc />
        public async Task<IEnumerable<GeneratedScheduleDto>> GetSchedules(DateTime startDate, DateTime endDate)
        {
            return await Mediator.Send(new GetSchedulesQueryCommand()
            {
                StartDate = startDate,
                EndDate = endDate,
            });
        }
    }
}
