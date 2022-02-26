using Ethos.Application.Commands.Validators;
using FluentValidation;

namespace Ethos.Application.Commands.Booking
{
    public class CreateBookingCommandValidator : EthosAbstractValidator<CreateBookingCommand>
    {
        public CreateBookingCommandValidator()
        {
            RuleFor(command => command.ScheduleId).NotEmpty();
            RuleFor(command => command.StartDate)
                .Must(BeUtc)
                .WithMessage(UtcMessage)
                .NotEmpty();
            RuleFor(command => command.EndDate)
                .Must(BeUtc)
                .WithMessage(UtcMessage)
                .NotEmpty();
        }
    }
}
