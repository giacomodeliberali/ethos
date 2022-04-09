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
                .NotEmpty();

            RuleFor(command => command.InstanceEndDate)
                .NotEmpty();

            RuleFor(command => command.StartDate)
                .NotEmpty();

            RuleFor(command => command.DurationInMinutes)
                .GreaterThan(0);
        }
    }
}
