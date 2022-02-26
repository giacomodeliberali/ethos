using Ethos.Application.Commands.Validators;
using FluentValidation;

namespace Ethos.Application.Commands.Schedules.Recurring
{
    public class UpdateRecurringScheduleInstanceCommandValidator : EthosAbstractValidator<UpdateRecurringScheduleInstanceCommand>
    {
        public UpdateRecurringScheduleInstanceCommandValidator()
        {
            RuleFor(command => command.Id)
                .NotEmpty();

            RuleFor(command => command.InstanceStartDate)
                .NotEmpty()
                .Must(BeUtc).WithMessage(UtcMessage);

            RuleFor(command => command.InstanceEndDate)
                .NotEmpty()
                .Must(BeUtc).WithMessage(UtcMessage);

            RuleFor(command => command.StartDate)
                .NotEmpty()
                .Must(BeUtc).WithMessage(UtcMessage);

            RuleFor(command => command.DurationInMinutes)
                .GreaterThan(0);
        }
    }
}
