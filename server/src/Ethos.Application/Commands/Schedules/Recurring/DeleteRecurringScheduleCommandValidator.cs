using Ethos.Application.Commands.Validators;
using FluentValidation;

namespace Ethos.Application.Commands.Schedules.Recurring
{
    public class DeleteRecurringScheduleCommandValidator : EthosAbstractValidator<DeleteRecurringScheduleCommand>
    {
        public DeleteRecurringScheduleCommandValidator()
        {
            RuleFor(command => command.Id).NotEmpty();

            RuleFor(command => command.InstanceStartDate)
                .NotEmpty();

            RuleFor(command => command.InstanceEndDate)
                .NotEmpty();
        }
    }
}
