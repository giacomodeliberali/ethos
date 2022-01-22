using FluentValidation;

namespace Ethos.Application.Commands.Validators
{
    public class UpdateScheduleCommandValidator : EthosAbstractValidator<UpdateSingleScheduleCommand>
    {
        public UpdateScheduleCommandValidator()
        {
            RuleFor(command => command.Id)
                .NotEmpty();

            RuleFor(command => command.StartDate)
                .NotEmpty()
                .Must(BeUtc).WithMessage(UtcMessage);

            RuleFor(command => command.DurationInMinutes)
                .GreaterThan(0);
        }
    }
}
