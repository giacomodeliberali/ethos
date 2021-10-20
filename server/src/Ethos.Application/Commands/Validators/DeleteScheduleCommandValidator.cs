using FluentValidation;

namespace Ethos.Application.Commands.Validators
{
    public class DeleteScheduleCommandValidator : EthosAbstractValidator<DeleteScheduleCommand>
    {
        public DeleteScheduleCommandValidator()
        {
            RuleFor(command => command.Id).NotEmpty();

            RuleFor(command => command.InstanceStartDate)
                .NotEmpty()
                .Must(BeUtc)
                .WithMessage(UtcMessage);

            RuleFor(command => command.InstanceEndDate)
                .NotEmpty()
                .Must(BeUtc)
                .WithMessage(UtcMessage);
        }
    }
}
