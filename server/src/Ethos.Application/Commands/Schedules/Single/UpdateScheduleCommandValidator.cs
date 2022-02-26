using Ethos.Application.Commands.Validators;
using FluentValidation;

namespace Ethos.Application.Commands.Schedules.Single
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
