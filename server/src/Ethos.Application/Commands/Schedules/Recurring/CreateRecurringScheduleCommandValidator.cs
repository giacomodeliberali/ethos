using Ethos.Application.Commands.Validators;
using FluentValidation;

namespace Ethos.Application.Commands.Schedules.Recurring
{
    public class CreateRecurringScheduleCommandValidator : EthosAbstractValidator<CreateRecurringScheduleCommand>
    {
        public CreateRecurringScheduleCommandValidator()
        {
            RuleFor(command => command.Name).NotEmpty();
            RuleFor(command => command.Description).NotEmpty();
            RuleFor(command => command.OrganizerId).NotEmpty();
            RuleFor(command => command.RecurringCronExpression).NotEmpty();
            RuleFor(command => command.DurationInMinutes).GreaterThan(0);
            RuleFor(command => command.StartDate)
                .NotEmpty();
            RuleFor(command => command.EndDate)
                .NotEmpty();
            
            RuleFor(command => command.TimeZone).NotEmpty();
        }
    }
}
