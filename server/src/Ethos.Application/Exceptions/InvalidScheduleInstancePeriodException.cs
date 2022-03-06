using System;
using Ethos.Domain.Exceptions;

namespace Ethos.Application.Exceptions;

public class InvalidScheduleInstancePeriodException : BusinessException
{
    public InvalidScheduleInstancePeriodException(DateTime instanceStartDate, DateTime instanceEndDate, int occurrence) 
        : base($"Schedule for period {instanceStartDate:s} - {instanceEndDate:s} has {occurrence} occurrences")
    {
    }
}