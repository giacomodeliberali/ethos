using Ethos.Domain.Exceptions;

namespace Ethos.Application.Exceptions
{
    public class InvalidOrganizerException : BusinessException
    {
        public InvalidOrganizerException()
            : base("The provided organizer id is either invalid or not an admin")
        {
        }
    }
}
