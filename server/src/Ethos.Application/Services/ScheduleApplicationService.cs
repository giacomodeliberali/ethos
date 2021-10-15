using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cronos;
using Ethos.Application.Commands;
using Ethos.Application.Contracts.Schedule;
using Ethos.Application.Identity;
using Ethos.Domain.Common;
using Ethos.Domain.Repositories;
using Ethos.Query.Services;
using Ethos.Shared;
using MediatR;

namespace Ethos.Application.Services
{
    public class ScheduleApplicationService : BaseApplicationService, IScheduleApplicationService
    {
        private readonly ICurrentUser _currentUser;
        private readonly IScheduleQueryService _scheduleQueryService;
        private readonly IBookingQueryService _bookingQueryService;
        private readonly IScheduleExceptionQueryService _scheduleExceptionQueryService;
        private readonly IMediator _mediator;

        public ScheduleApplicationService(
            IUnitOfWork unitOfWork,
            IGuidGenerator guidGenerator,
            ICurrentUser currentUser,
            IScheduleQueryService scheduleQueryService,
            IBookingQueryService bookingQueryService,
            IScheduleExceptionQueryService scheduleExceptionQueryService,
            IMediator mediator)
            : base(unitOfWork, guidGenerator)
        {
            _currentUser = currentUser;
            _scheduleQueryService = scheduleQueryService;
            _bookingQueryService = bookingQueryService;
            _scheduleExceptionQueryService = scheduleExceptionQueryService;
            _mediator = mediator;
        }

        /// <inheritdoc />
        public async Task<CreateScheduleReplyDto> CreateAsync(CreateScheduleRequestDto input)
        {
            var scheduleId = await _mediator.Send(new CreateScheduleCommand(
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
            await _mediator.Send(new UpdateScheduleCommand()
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
            await _mediator.Send(new DeleteScheduleCommand
            {
                Id = input.Id,
                InstanceStartDate = input.InstanceStartDate,
                InstanceEndDate = input.InstanceEndDate,
                RecurringScheduleOperationType = input.RecurringScheduleOperationType,
            });
        }

        /// <inheritdoc />
        public async Task<IEnumerable<GeneratedScheduleDto>> GetSchedules(DateTime from, DateTime to)
        {
            var startDateStartOfDay = from.Date.ToUniversalTime();
            var endDateEndOfDay = to.Date.AddDays(1).AddTicks(-1).ToUniversalTime();

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
