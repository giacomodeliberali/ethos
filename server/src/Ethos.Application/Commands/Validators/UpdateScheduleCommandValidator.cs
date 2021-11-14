using FluentValidation;

namespace Ethos.Application.Commands.Validators
{
    public class UpdateScheduleCommandValidator : EthosAbstractValidator<UpdateScheduleCommand>
    {
        public UpdateScheduleCommandValidator()
        {
            RuleFor(command => command.Id).NotEmpty();
            RuleFor(command => command.InstanceStartDate)
                .Must(BeUtc)
                .WithMessage(UtcMessage)
                .NotEmpty();
            RuleFor(command => command.InstanceEndDate)
                .Must(BeUtc)
                .WithMessage(UtcMessage)
                .NotEmpty();
        }
    }
}
