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
        public async Task UpdateSingleAsync(UpdateSingleScheduleRequestDto input)
        {
            await Mediator.Send(new UpdateSingleScheduleCommand()
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
        public async Task UpdateRecurringInstanceAsync(UpdateRecurringScheduleInstanceRequestDto input)
        {
            await Mediator.Send(new UpdateRecurringScheduleInstanceCommand()
            {
                Id = input.Id,
                Name = input.Name,
                Description = input.Description,
                StartDate = input.StartDate,
                DurationInMinutes = input.DurationInMinutes,
                OrganizerId = input.OrganizerId,
                ParticipantsMaxNumber = input.ParticipantsMaxNumber,
                InstanceStartDate = input.InstanceStartDate,
                InstanceEndDate = input.InstanceEndDate,
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
