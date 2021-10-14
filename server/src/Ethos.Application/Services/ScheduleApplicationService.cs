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
using Ethos.Domain.Guards;
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

            if (organizer == null || !await _userManager.IsInRoleAsync(organizer, RoleConstants.Admin))
            {
                throw new BusinessException("Invalid organizer id");
            }

            Guard.Against.Null(input.StartDate, nameof(input.StartDate));
            Guard.Against.Null(input.EndDate, nameof(input.EndDate));

            var createdScheduleId = GuidGenerator.Create();
            if (string.IsNullOrEmpty(input.RecurringCronExpression))
            {
                var schedule = SingleSchedule.Factory.Create(
                    createdScheduleId,
                    organizer,
                    input.Name,
                    input.Description,
                    input.ParticipantsMaxNumber,
                    new Period(input.StartDate.Value, input.EndDate.Value));

                await _scheduleRepository.CreateAsync(schedule);
            }
            else
            {
                Guard.Against.NegativeOrZero(input.DurationInMinutes, nameof(input.DurationInMinutes));

                var schedule = RecurringSchedule.Factory.Create(
                    createdScheduleId,
                    organizer,
                    input.Name,
                    input.Description,
                    input.ParticipantsMaxNumber,
                    new Period(input.StartDate.Value, input.EndDate.Value),
                    input.DurationInMinutes,
                    input.RecurringCronExpression);

                await _scheduleRepository.CreateAsync(schedule);
            }

            await UnitOfWork.SaveChangesAsync();

            return new CreateScheduleReplyDto()
            {
                Id = createdScheduleId,
            };
        }

        /// <inheritdoc />
        public async Task UpdateAsync(UpdateScheduleRequestDto input)
        {
            var schedule = await _scheduleRepository.GetByIdAsync(input.Id);

            if (schedule is RecurringSchedule recurringSchedule)
            {
                Guard.Against.Null(input.InstanceEndDate, nameof(input.InstanceStartDate));
                Guard.Against.Null(input.InstanceEndDate, nameof(input.InstanceEndDate));
                Guard.Against.NotUtc(input.InstanceEndDate, nameof(input.InstanceEndDate));
                Guard.Against.NotUtc(input.InstanceEndDate, nameof(input.InstanceEndDate));
                Guard.Against.Null(input.RecurringScheduleOperationType, nameof(input.RecurringScheduleOperationType));

                await _mediator.Send(new UpdateRecurringScheduleCommand(input, recurringSchedule));
            }
            else if (schedule is SingleSchedule singleSchedule)
            {
                await _mediator.Send(new UpdateSingleScheduleCommand(input, singleSchedule));
            }

            await UnitOfWork.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task DeleteAsync(DeleteScheduleRequestDto input)
        {
            var schedule = await _scheduleRepository.GetByIdAsync(input.Id);

            if (schedule is RecurringSchedule recurringSchedule)
            {
                Guard.Against.Null(input.RecurringScheduleOperationType, nameof(input.RecurringScheduleOperationType));
                Guard.Against.Null(input.InstanceStartDate, nameof(input.InstanceStartDate));
                Guard.Against.Null(input.InstanceEndDate, nameof(input.InstanceEndDate));
                Guard.Against.NotUtc(input.InstanceEndDate, nameof(input.InstanceEndDate));
                Guard.Against.NotUtc(input.InstanceEndDate, nameof(input.InstanceEndDate));

                await _mediator.Send(new DeleteRecurringScheduleCommand(
                    recurringSchedule,
                    input.RecurringScheduleOperationType.Value,
                    input.InstanceStartDate.Value,
                    input.InstanceEndDate.Value));
            }
            else if (schedule is SingleSchedule singleSchedule)
            {
                await _mediator.Send(new DeleteSingleScheduleCommand(singleSchedule));
            }

            await UnitOfWork.SaveChangesAsync();
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
