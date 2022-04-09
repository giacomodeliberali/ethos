using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ethos.Application.Commands.Schedules.Recurring;
using Ethos.Application.Contracts;
using Ethos.Application.Exceptions;
using Ethos.Common;
using Ethos.Domain.Common;
using Ethos.Domain.Entities;
using Ethos.Domain.Exceptions;
using Ethos.Domain.Repositories;
using Ethos.Query.Services;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Ethos.Application.Handlers.Schedules.Recurring
{
    public class UpdateRecurringScheduleInstanceCommandHandler : AsyncRequestHandler<UpdateRecurringScheduleInstanceCommand>
    {
        private readonly IScheduleRepository _scheduleRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IBookingQueryService _bookingQueryService;
        private readonly IScheduleExceptionRepository _scheduleExceptionRepository;
        private readonly IGuidGenerator _guidGenerator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateRecurringScheduleInstanceCommandHandler> _logger;

        public UpdateRecurringScheduleInstanceCommandHandler(
            IScheduleRepository scheduleRepository,
            UserManager<ApplicationUser> userManager,
            IBookingQueryService bookingQueryService,
            IScheduleExceptionRepository scheduleExceptionRepository,
            IGuidGenerator guidGenerator,
            IUnitOfWork unitOfWork,
            ILogger<UpdateRecurringScheduleInstanceCommandHandler> logger)
        {
            _scheduleRepository = scheduleRepository;
            _userManager = userManager;
            _bookingQueryService = bookingQueryService;
            _scheduleExceptionRepository = scheduleExceptionRepository;
            _guidGenerator = guidGenerator;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        protected override async Task Handle(UpdateRecurringScheduleInstanceCommand request, CancellationToken cancellationToken)
        {
            var schedule = await _scheduleRepository.GetByIdAsync(request.Id);

            if (schedule is SingleSchedule)
            {
                throw new BusinessException("Non è possibile aggiornare un corso singolo");
            }

            var recurringSchedule = (RecurringSchedule)schedule;
            
            var occurrences = recurringSchedule.GetOccurrences(
                new DateOnlyPeriod(request.InstanceStartDate, request.InstanceEndDate),
                schedule.TimeZone);

            if (occurrences.Count() != 1)
            {
                throw new InvalidScheduleInstancePeriodException(request.InstanceStartDate, request.InstanceEndDate, occurrences.Count());
            }

            if (request.RecurringScheduleOperationType == RecurringScheduleOperationType.Instance)
            {
                await UpdateRecurringScheduleInstance(recurringSchedule, request);    
            }
            else if (request.RecurringScheduleOperationType == RecurringScheduleOperationType.InstanceAndFuture)
            {
                await UpdateRecurringScheduleInstanceAndFuture(recurringSchedule, request);
            }

            await _unitOfWork.SaveChangesAsync();
        }

        /// <summary>
        /// Create a new exception for the modified instance and create a new single instance.
        /// </summary>
        private async Task UpdateRecurringScheduleInstance(
            RecurringSchedule schedule, 
            UpdateRecurringScheduleInstanceCommand request)
        {
            var existingBookings = await _bookingQueryService.GetAllBookingsInRange(
                schedule.Id,
                new DateOnlyPeriod(request.InstanceStartDate, request.InstanceEndDate));

            if (existingBookings.Any())
            {
                throw new CanNotEditScheduleWithExistingBookingsException(existingBookings.Count);
            }

            var scheduleException = ScheduleException.Factory.Create(
                _guidGenerator.Create(),
                schedule,
                request.InstanceStartDate.ToDateTimeOffset(schedule.TimeZone),
                request.InstanceEndDate.ToDateTimeOffset(schedule.TimeZone));
            
            _logger.LogDebug("[ScheduleException] Created for {StartDate} - {EndDate}", scheduleException.StartDate, scheduleException.EndDate);

            await _scheduleExceptionRepository.CreateAsync(scheduleException);

            var organizer = await GetOrganizer(request.OrganizerId);
            
            var newSingleInstance = SingleSchedule.Factory.Create(
                _guidGenerator.Create(),
                organizer,
                request.Name,
                request.Description,
                request.ParticipantsMaxNumber,
                request.StartDate.ToDateTimeOffset(schedule.TimeZone), 
                request.DurationInMinutes,
                schedule.TimeZone);
            
            _logger.LogDebug("[SingleSchedule] Created for {StartDate} - {EndDate}", newSingleInstance.StartDate, newSingleInstance.EndDate);

            await _scheduleRepository.CreateAsync(newSingleInstance);
        }

        private async Task UpdateRecurringScheduleInstanceAndFuture(
            RecurringSchedule schedule, 
            UpdateRecurringScheduleInstanceCommand request)
        {
            var existingBookings = await _bookingQueryService.GetAllBookingsInRange(
                schedule.Id,
                new DateOnlyPeriod(request.InstanceStartDate, schedule.Period.EndDate));

            if (existingBookings.Any())
            {
                throw new CanNotEditScheduleWithExistingBookingsException(existingBookings.Count);
            }
            
            // update past
            schedule.UpdateDate(
                new DateOnlyPeriod(schedule.Period.StartDate, request.InstanceStartDate), 
                schedule.DurationInMinutes, 
                schedule.RecurringCronExpressionString,
                schedule.TimeZone); 

            await _scheduleRepository.UpdateAsync(schedule);
            
            // create new future recurring with new info
            var organizer = await GetOrganizer(request.OrganizerId);

            var newRecurring = RecurringSchedule.Factory.Create(
                _guidGenerator.Create(),
                organizer,
                request.Name,
                request.Description,
                request.ParticipantsMaxNumber,
                new DateOnlyPeriod(request.StartDate, request.EndDate),
                request.DurationInMinutes,
                request.RecurringCronExpression,
                schedule.TimeZone);

            await _scheduleRepository.CreateAsync(newRecurring);
        }

        private async Task<ApplicationUser> GetOrganizer(Guid organizerId)
        {
            var organizer = await _userManager.FindByIdAsync(organizerId.ToString());

            if (organizer == null || !await _userManager.IsInRoleAsync(organizer, RoleConstants.Admin))
            {
                throw new BusinessException("Invalid organizer id");
            }

            return organizer;
        }
    }
}
