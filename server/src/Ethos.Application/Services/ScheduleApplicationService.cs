using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ethos.Application.Commands.Schedules.Recurring;
using Ethos.Application.Commands.Schedules.Single;
using Ethos.Application.Contracts.Schedule;
using Ethos.Application.Queries;
using MediatR;

namespace Ethos.Application.Services
{
    /// <summary>
    /// Contains the use cases for the web UI.
    /// </summary>
    public class ScheduleApplicationService : IScheduleApplicationService
    {
        private readonly IMediator _mediator;

        public ScheduleApplicationService(
            IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <inheritdoc />
        public async Task<CreateScheduleReplyDto> CreateAsync(CreateSingleScheduleRequestDto input)
        {
            var scheduleId = await _mediator.Send(new CreateSingleScheduleCommand(
                input.Name,
                input.Description,
                input.StartDate,
                input.DurationInMinutes,
                input.ParticipantsMaxNumber,
                input.OrganizerId,
                input.TimeZone));

            return new CreateScheduleReplyDto()
            {
                Id = scheduleId,
            };
        }

        public async Task<CreateScheduleReplyDto> CreateRecurringAsync(CreateRecurringScheduleRequestDto input)
        {
            var scheduleId = await _mediator.Send(new CreateRecurringScheduleCommand(
                input.Name,
                input.Description,
                input.StartDate,
                input.EndDate,
                input.DurationInMinutes,
                input.RecurringCronExpression,
                input.ParticipantsMaxNumber,
                input.OrganizerId,
                input.TimeZone));

            return new CreateScheduleReplyDto()
            {
                Id = scheduleId,
            };
        }

        /// <inheritdoc />
        public async Task UpdateSingleAsync(UpdateSingleScheduleRequestDto input)
        {
            await _mediator.Send(new UpdateSingleScheduleCommand()
            {
                Id = input.Id,
                Name = input.Name,
                Description = input.Description,
                StartDate = input.StartDate,
                DurationInMinutes = input.DurationInMinutes,
                OrganizerId = input.OrganizerId,
                ParticipantsMaxNumber = input.ParticipantsMaxNumber,
            });
        }

        /// <inheritdoc />
        public async Task UpdateRecurringAsync(UpdateRecurringScheduleRequestDto input)
        {
            await _mediator.Send(new UpdateRecurringScheduleInstanceCommand(
                input.Id,
                input.Name,
                input.Description,
                input.StartDate,
                input.EndDate,
                input.DurationInMinutes,
                input.RecurringCronExpression,
                input.OrganizerId,
                input.ParticipantsMaxNumber,
                input.InstanceStartDate,
                input.InstanceEndDate,
                input.RecurringScheduleOperationType));
        }

        public async Task DeleteRecurringAsync(DeleteRecurringScheduleRequestDto input)
        {
            await _mediator.Send(new DeleteRecurringScheduleCommand
            {
                Id = input.Id,
                InstanceStartDate = input.InstanceStartDate,
                InstanceEndDate = input.InstanceEndDate,
                RecurringScheduleOperationType = input.RecurringScheduleOperationType,
            });
        }

        public async Task<IEnumerable<GeneratedScheduleDto>> GetSchedules(DateTimeOffset startDate,
            DateTimeOffset endDate)
        {
            return await _mediator.Send(new GetSchedulesQuery(startDate, endDate));
        }

        /// <inheritdoc />
        public async Task DeleteAsync(DeleteSingleScheduleRequestDto input)
        {
            await _mediator.Send(new DeleteSingleScheduleCommand
            {
                Id = input.Id,
            });
        }

        /// <inheritdoc />
        public async Task<IEnumerable<RecurringScheduleDto>> GetAllRecurring()
        {
            return await _mediator.Send(new GetAllRecurringSchedulesQuery());
        }
    }
}
