using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using AutoMapper;
using Cronos;
using Ethos.Application.Commands;
using Ethos.Application.Contracts.Schedule;
using Ethos.Application.Identity;
using Ethos.Domain.Common;
using Ethos.Domain.Entities;
using Ethos.Domain.Exceptions;
using Ethos.Domain.Repositories;
using Ethos.Query.Services;
using Ethos.Shared;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Ethos.Application.Services
{
    public class ScheduleApplicationService : BaseApplicationService, IScheduleApplicationService
    {
        private readonly IScheduleRepository _scheduleRepository;
        private readonly ICurrentUser _currentUser;
        private readonly IScheduleQueryService _scheduleQueryService;
        private readonly IBookingQueryService _bookingQueryService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IScheduleExceptionRepository _scheduleExceptionRepository;
        private readonly IScheduleExceptionQueryService _scheduleExceptionQueryService;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public ScheduleApplicationService(
            IUnitOfWork unitOfWork,
            IGuidGenerator guidGenerator,
            IScheduleRepository scheduleRepository,
            ICurrentUser currentUser,
            IScheduleQueryService scheduleQueryService,
            IBookingQueryService bookingQueryService,
            UserManager<ApplicationUser> userManager,
            IScheduleExceptionRepository scheduleExceptionRepository,
            IScheduleExceptionQueryService scheduleExceptionQueryService,
            IMapper mapper,
            IMediator mediator)
            : base(unitOfWork, guidGenerator)
        {
            _scheduleRepository = scheduleRepository;
            _currentUser = currentUser;
            _scheduleQueryService = scheduleQueryService;
            _bookingQueryService = bookingQueryService;
            _userManager = userManager;
            _scheduleExceptionRepository = scheduleExceptionRepository;
            _scheduleExceptionQueryService = scheduleExceptionQueryService;
            _mapper = mapper;
            _mediator = mediator;
        }

        /// <inheritdoc />
        public async Task<CreateScheduleReplyDto> CreateAsync(CreateScheduleRequestDto input)
        {
            var organizer = await _userManager.FindByIdAsync(input.OrganizerId.ToString());

            if (organizer == null)
            {
                throw new BusinessException("Invalid organizer id");
            }

            if (!await _userManager.IsInRoleAsync(organizer, RoleConstants.Admin))
            {
                throw new BusinessException("The organizer is not an admin");
            }

            Schedule schedule;

            if (string.IsNullOrEmpty(input.RecurringCronExpression))
            {
                Guard.Against.Null(input.StartDate, nameof(input.StartDate));
                Guard.Against.Null(input.EndDate, nameof(input.EndDate));

                schedule = SingleSchedule.Factory.Create(
                    GuidGenerator.Create(),
                    organizer,
                    input.Name,
                    input.Description,
                    input.ParticipantsMaxNumber,
                    input.StartDate.Value,
                    input.EndDate.Value);
            }
            else
            {
                Guard.Against.Null(input.StartDate, nameof(input.StartDate));
                Guard.Against.NegativeOrZero(input.DurationInMinutes, nameof(input.DurationInMinutes));
                Guard.Against.Null(input.RecurringCronExpression, nameof(input.RecurringCronExpression));

                schedule = RecurringSchedule.Factory.Create(
                    GuidGenerator.Create(),
                    organizer,
                    input.Name,
                    input.Description,
                    input.ParticipantsMaxNumber,
                    input.StartDate.Value,
                    input.EndDate,
                    input.DurationInMinutes,
                    input.RecurringCronExpression);
            }

            await _scheduleRepository.CreateAsync(schedule);

            await UnitOfWork.SaveChangesAsync();

            return new CreateScheduleReplyDto()
            {
                Id = schedule.Id,
            };
        }

        /// <inheritdoc />
        public async Task UpdateAsync(UpdateScheduleRequestDto input)
        {
            var schedule = await _scheduleRepository.GetByIdAsync(input.Id);

            if (schedule is RecurringSchedule recurringSchedule)
            {
                await _mediator.Send(new UpdateRecurringScheduleCommand(input, recurringSchedule));
            }
            else if (schedule is SingleSchedule singleSchedule)
            {
                await _mediator.Send(new UpdateSingleScheduleCommand(input, singleSchedule));
            }
            else
            {
                throw new ArgumentException("Invalid schedule");
            }
        }

        /// <inheritdoc />
        public async Task DeleteAsync(DeleteScheduleRequestDto input)
        {
            var schedule = await _scheduleRepository.GetByIdAsync(input.Id);

            if (schedule is RecurringSchedule recurringSchedule)
            {
                Guard.Against.Null(input.RecurringScheduleOperationType, nameof(input.RecurringScheduleOperationType));
                Guard.Against.Default(input.InstanceStartDate, nameof(input.InstanceStartDate));
                Guard.Against.Default(input.InstanceEndDate, nameof(input.InstanceEndDate));

                await DeleteRecurringSchedule(
                    recurringSchedule,
                    input.RecurringScheduleOperationType.Value,
                    input.InstanceStartDate,
                    input.InstanceEndDate);
            }
            else if (schedule is SingleSchedule singleSchedule)
            {
                await DeleteSingleSchedule(singleSchedule);
            }

            await UnitOfWork.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<GeneratedScheduleDto>> GetSchedules(DateTime from, DateTime to)
        {
            var recurringSchedules = await _scheduleQueryService.GetOverlappingRecurringSchedulesAsync(from, to);

            var singleSchedules = await _scheduleQueryService.GetOverlappingSingleSchedulesAsync(from, to);

            var isAdmin = await _currentUser.IsInRole(RoleConstants.Admin);

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

            foreach (var schedule in recurringSchedules)
            {
                var cronExpression = CronExpression.Parse(schedule.RecurringExpression);

                var scheduleExceptions = await _scheduleExceptionQueryService.GetScheduleExceptionsAsync(schedule.Id, from, to);

                var nextExecutions = cronExpression.GetOccurrences(
                    fromUtc: schedule.StartDate >= from ? schedule.StartDate.ToUniversalTime() : from,
                    toUtc: schedule.EndDate.HasValue && schedule.EndDate.Value <= to ? schedule.EndDate.Value.ToUniversalTime() : to.ToUniversalTime(),
                    zone: TimeZoneInfo.Local,
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

        private async Task DeleteRecurringSchedule(
            RecurringSchedule schedule,
            RecurringScheduleOperationType operationType,
            DateTime startDate,
            DateTime endDate)
        {
            if (operationType == RecurringScheduleOperationType.Future)
            {
                var futureBookings = await _bookingQueryService.GetAllBookingsInRange(
                    schedule.Id,
                    startDate: endDate.AddMilliseconds(1),
                    endDate: DateTime.MaxValue);

                if (futureBookings.Any())
                {
                    throw new BusinessException(
                        $"Non è possibile eliminare la schedulazione, sono già presenti {futureBookings.Count} prenotazioni");
                }

                var lastPastOccurrenceEnd = schedule.RecurringCronExpression.GetOccurrences(
                        fromUtc: schedule.StartDate,
                        toUtc: startDate,
                        toInclusive: false)
                    .Last()
                    .AddMinutes(schedule.DurationInMinutes);

                schedule.UpdateDateTime(
                    schedule.StartDate,
                    lastPastOccurrenceEnd,
                    schedule.DurationInMinutes,
                    schedule.RecurringCronExpressionString);

                await _scheduleRepository.UpdateAsync(schedule);
            }
            else if (operationType == RecurringScheduleOperationType.Instance)
            {
                // add to exception table
                var existingBookings = await _bookingQueryService.GetAllBookingsInRange(
                    schedule.Id,
                    startDate,
                    endDate);

                if (existingBookings.Any())
                {
                    throw new BusinessException(
                        $"Non è possibile eliminare la schedulazione, sono presenti {existingBookings.Count} prenotazioni");
                }

                var scheduleException = ScheduleException.Factory.Create(
                    GuidGenerator.Create(),
                    schedule,
                    startDate,
                    endDate);

                await _scheduleExceptionRepository.CreateAsync(scheduleException);
            }

            await UnitOfWork.SaveChangesAsync();
        }

        private async Task DeleteSingleSchedule(SingleSchedule schedule)
        {
            var existingBookings = await _bookingQueryService.GetAllBookingsInRange(
                schedule.Id,
                schedule.StartDate,
                schedule.EndDate);

            if (existingBookings.Any())
            {
                throw new BusinessException(
                    $"Non è possibile eliminare la schedulazione, sono presenti {existingBookings.Count} prenotazioni");
            }

            await _scheduleRepository.DeleteAsync(schedule);
        }
    }
}
