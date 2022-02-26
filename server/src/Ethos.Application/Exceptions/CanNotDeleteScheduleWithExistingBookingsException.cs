using Ethos.Domain.Exceptions;

namespace Ethos.Application.Exceptions
{
    public class CanNotDeleteScheduleWithExistingBookingsException : BusinessException
    {
        public CanNotDeleteScheduleWithExistingBookingsException(int existingBookingCount)
            : base($"Non è possibile eliminare la schedulazione, sono presenti {existingBookingCount} prenotazioni")
        {
        }
    }
}
