using System;
using FluentValidation;

namespace Ethos.Application.Commands.Validators
{
    public class EthosAbstractValidator<T> : AbstractValidator<T>
    {
        protected const string UtcMessage = "{PropertyName} is not in UTC";

        protected bool BeUtc(DateTime dateTime)
        {
            return dateTime.Kind == DateTimeKind.Utc;
        }

        protected bool BeUtc(DateTime? dateTime)
        {
            return !dateTime.HasValue || dateTime.Value.Kind == DateTimeKind.Utc;
        }
    }
}
