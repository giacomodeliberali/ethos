using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ethos.Application.Commands.Schedule.Single;
using Ethos.Application.Exceptions;
using Ethos.Domain.Common;
using Ethos.Domain.Entities;
using Ethos.Domain.Exceptions;
using Ethos.Domain.Repositories;
using Ethos.Query.Services;
using Ethos.Shared;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Ethos.Application.Handlers.Schedule.Single
{
    public class UpdateSingleScheduleCommandHandler : AsyncRequestHandler<UpdateSingleScheduleCommand>
    {
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IBookingQueryService _bookingQueryService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateSingleScheduleCommandHandler(
            IScheduleRepository scheduleRepository,
            IBookingQueryService bookingQueryService,
            UserManager<ApplicationUser> userManager,
            IUnitOfWork unitOfWork)
        {
            _scheduleRepository = scheduleRepository;
            _bookingQueryService = bookingQueryService;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        protected override async Task Handle(UpdateSingleScheduleCommand request, CancellationToken cancellationToken)
        {
            var schedule = await _scheduleRepository.GetByIdAsync(request.Id);

            if (schedule is RecurringSchedule)
            {
                throw new BusinessException("Non è possibile aggiornare un corso ricorrente");
            }

            await UpdateSchedule(schedule as SingleSchedule, request);
        }

        private async Task UpdateSchedule(SingleSchedule schedule, UpdateSingleScheduleCommand request)
        {
            var existingBookings = await _bookingQueryService.GetAllBookingsInRange(
                schedule.Id,
                schedule.Period.StartDate,
                schedule.Period.EndDate);

            if (existingBookings.Any())
            {
                throw new CanNotEditScheduleWithExistingBookingsException(existingBookings.Count);
            }

            var organizer = await _userManager.FindByIdAsync(request.OrganizerId.ToString());

            if (organizer == null || !await _userManager.IsInRoleAsync(organizer, RoleConstants.Admin))
            {
                throw new InvalidOrganizerException();
            }

            schedule!.UpdateOrganizer(organizer);
            schedule.UpdateNameAndDescription(request.Name, request.Description);
            schedule.UpdateParticipantsMaxNumber(request.ParticipantsMaxNumber);
            schedule.UpdatePeriod(new Period(request.StartDate, request.DurationInMinutes));

            await _scheduleRepository.UpdateAsync(schedule);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
