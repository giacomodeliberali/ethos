using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ethos.Application.Commands.Schedule.Recurring;
using Ethos.Application.Exceptions;
using Ethos.Domain.Common;
using Ethos.Domain.Entities;
using Ethos.Domain.Exceptions;
using Ethos.Domain.Repositories;
using Ethos.Query.Services;
using Ethos.Shared;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Ethos.Application.Handlers.Schedule.Recurring
{
    public class UpdateRecurringScheduleInstanceCommandHandler : AsyncRequestHandler<UpdateRecurringScheduleInstanceCommand>
    {
        private readonly IScheduleRepository _scheduleRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IBookingQueryService _bookingQueryService;
        private readonly IScheduleExceptionRepository _scheduleExceptionRepository;
        private readonly IGuidGenerator _guidGenerator;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateRecurringScheduleInstanceCommandHandler(
            IScheduleRepository scheduleRepository,
            UserManager<ApplicationUser> userManager,
            IBookingQueryService bookingQueryService,
            IScheduleExceptionRepository scheduleExceptionRepository,
            IGuidGenerator guidGenerator,
            IUnitOfWork unitOfWork)
        {
            _scheduleRepository = scheduleRepository;
            _userManager = userManager;
            _bookingQueryService = bookingQueryService;
            _scheduleExceptionRepository = scheduleExceptionRepository;
            _guidGenerator = guidGenerator;
            _unitOfWork = unitOfWork;
        }

        protected override async Task Handle(UpdateRecurringScheduleInstanceCommand request, CancellationToken cancellationToken)
        {
            var schedule = await _scheduleRepository.GetByIdAsync(request.Id);

            if (schedule is SingleSchedule)
            {
                throw new BusinessException("Non è possibile aggiornare un corso singolo");
            }

            await UpdateRecurringScheduleInstance(schedule as RecurringSchedule, request);
        }

        /// <summary>
        /// Create a new exception for the modified instance and create a new single instance.
        /// </summary>
        private async Task UpdateRecurringScheduleInstance(RecurringSchedule schedule, UpdateRecurringScheduleInstanceCommand request)
        {
            var organizer = await _userManager.FindByIdAsync(request.OrganizerId.ToString());

            if (organizer == null || !await _userManager.IsInRoleAsync(organizer, RoleConstants.Admin))
            {
                throw new BusinessException("Invalid organizer id");
            }

            var existingBookings = await _bookingQueryService.GetAllBookingsInRange(
                schedule.Id,
                request.InstanceStartDate,
                request.InstanceEndDate);

            if (existingBookings.Any())
            {
                throw new CanNotEditScheduleWithExistingBookingsException(existingBookings.Count);
            }

            var scheduleException = ScheduleException.Factory.Create(
                _guidGenerator.Create(),
                schedule,
                request.InstanceStartDate,
                request.InstanceEndDate);

            await _scheduleExceptionRepository.CreateAsync(scheduleException);

            var newSingleInstance = SingleSchedule.Factory.Create(
                _guidGenerator.Create(),
                organizer,
                request.Name,
                request.Description,
                request.ParticipantsMaxNumber,
                new Period(request.StartDate, request.DurationInMinutes));

            await _scheduleRepository.CreateAsync(newSingleInstance);

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
