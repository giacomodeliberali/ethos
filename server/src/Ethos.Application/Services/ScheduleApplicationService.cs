using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using AutoMapper;
using Cronos;
using Ethos.Application.Contracts.Schedule;
using Ethos.Application.Identity;
using Ethos.Domain.Common;
using Ethos.Domain.Entities;
using Ethos.Domain.Exceptions;
using Ethos.Domain.Repositories;
using Ethos.Query.Services;
using Ethos.Shared;
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
        private readonly IMapper _mapper;

        public ScheduleApplicationService(
            IUnitOfWork unitOfWork,
            IGuidGenerator guidGenerator,
            IScheduleRepository scheduleRepository,
            ICurrentUser currentUser,
            IScheduleQueryService scheduleQueryService,
            IBookingQueryService bookingQueryService,
            UserManager<ApplicationUser> userManager,
            IMapper mapper)
        : base(unitOfWork, guidGenerator)
        {
            _scheduleRepository = scheduleRepository;
            _currentUser = currentUser;
            _scheduleQueryService = scheduleQueryService;
            _bookingQueryService = bookingQueryService;
            _userManager = userManager;
            _mapper = mapper;
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

                schedule = Schedule.Factory.CreateNonRecurring(
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

                schedule = Schedule.Factory.CreateRecurring(
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

            var organizer = await _userManager.FindByIdAsync(input.OrganizerId.ToString());

            if (organizer == null)
            {
                throw new BusinessException("Invalid organizer id");
            }

            if (!await _userManager.IsInRoleAsync(organizer, RoleConstants.Admin))
            {
                throw new BusinessException("The organizer is not an admin");
            }

            schedule
                .UpdateOrganizer(organizer)
                .UpdateNameAndDescription(input.Name, input.Description, input.ParticipantsMaxNumber)
                .UpdateDateTime(
                    input.StartDate!.Value,
                    input.EndDate,
                    input.DurationInMinutes,
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
            var schedules = await _scheduleQueryService.GetOverlappingSchedulesAsync(from, to);

            var isAdmin = await _currentUser.IsInRole(RoleConstants.Admin);

            var result = new List<GeneratedScheduleDto>();

            foreach (var schedule in schedules)
            {
                if (string.IsNullOrEmpty(schedule.RecurringExpression))
                {
                    var startDate = schedule.StartDate;
                    var endDate = schedule.StartDate.Add(TimeSpan.FromMinutes(schedule.DurationInMinutes));
                    var bookings = await _bookingQueryService.GetAllInScheduleInRange(schedule.Id, startDate, endDate);

                    result.Add(new GeneratedScheduleDto()
                    {
                        ScheduleId = schedule.Id,
                        Name = schedule.Name,
                        Description = schedule.Description,
                        ParticipantsMaxNumber = schedule.ParticipantsMaxNumber,
                        StartDate = startDate,
                        EndDate = endDate,
                        Organizer = new GeneratedScheduleDto.UserDto()
                        {
                            Id = schedule.OrganizerId,
                            FullName = schedule.OrganizerFullName,
                            Email = schedule.OrganizerEmail,
                            UserName = schedule.OrganizerUserName,
                        },
                        Bookings = bookings.Select(b => new GeneratedScheduleDto.BookingDto()
                        {
                            Id = b.Id,
                            User = isAdmin ? new GeneratedScheduleDto.UserDto()
                            {
                                Id = b.UserId,
                                FullName = b.UserFullName,
                                Email = b.UserEmail,
                                UserName = b.UserName,
                            }
                                : null,
                        }),
                    });
                    continue;
                }

                var cronExpression = CronExpression.Parse(schedule.RecurringExpression);

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

                    var bookings = await _bookingQueryService.GetAllInScheduleInRange(schedule.Id, startDate, endDate);

                    result.Add(new GeneratedScheduleDto()
                    {
                        ScheduleId = schedule.Id,
                        Name = schedule.Name,
                        Description = schedule.Description,
                        ParticipantsMaxNumber = schedule.ParticipantsMaxNumber,
                        StartDate = startDate,
                        EndDate = endDate,
                        Organizer = new GeneratedScheduleDto.UserDto()
                        {
                            Id = schedule.OrganizerId,
                            FullName = schedule.OrganizerFullName,
                            Email = schedule.OrganizerEmail,
                            UserName = schedule.OrganizerUserName,
                        },
                        Bookings = bookings.Select(b => new GeneratedScheduleDto.BookingDto()
                        {
                            Id = b.Id,
                            User = isAdmin ? new GeneratedScheduleDto.UserDto()
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
    }
}
