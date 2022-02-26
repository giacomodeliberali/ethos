using Ethos.Application.Commands.Validators;
using FluentValidation;

namespace Ethos.Application.Commands.Schedule.Recurring
{
    public class DeleteRecurringScheduleCommandValidator : EthosAbstractValidator<DeleteRecurringScheduleCommand>
    {
        public DeleteRecurringScheduleCommandValidator()
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
