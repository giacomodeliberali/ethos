using FluentValidation;

namespace Ethos.Application.Commands.Validators
{
    public class CreateScheduleCommandValidator : EthosAbstractValidator<CreateScheduleCommand>
    {
        public CreateScheduleCommandValidator()
        {
            RuleFor(command => command.Name).NotEmpty();
            RuleFor(command => command.Description).NotEmpty();
            RuleFor(command => command.OrganizerId).NotEmpty();
            RuleFor(command => command.StartDate).Must(BeUtc).WithMessage(UtcMessage);
            RuleFor(command => command.EndDate).Must(BeUtc).WithMessage(UtcMessage);
        }
    }
}
