using FluentValidation;

namespace Ethos.Application.Commands.Validators
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
