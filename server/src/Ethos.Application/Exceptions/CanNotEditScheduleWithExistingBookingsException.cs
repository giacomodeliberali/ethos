using Ethos.Domain.Exceptions;

namespace Ethos.Application.Exceptions
{
    public class CanNotEditScheduleWithExistingBookingsException : BusinessException
    {
        public CanNotEditScheduleWithExistingBookingsException(int existingBookingCount)
            : base($"Non è possibile modificare la schedulazione, sono presenti {existingBookingCount} prenotazioni")
        {
        }
    }
}
