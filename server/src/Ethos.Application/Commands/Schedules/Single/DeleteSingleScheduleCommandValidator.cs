using Ethos.Application.Commands.Validators;
using FluentValidation;

namespace Ethos.Application.Commands.Schedules.Single
{
    public class DeleteSingleScheduleCommandValidator : EthosAbstractValidator<DeleteSingleScheduleCommand>
    {
        public DeleteSingleScheduleCommandValidator()
        {
            RuleFor(command => command.Id).NotEmpty();
        }
    }
}
