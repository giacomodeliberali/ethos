namespace Ethos.Domain.Exceptions
{
    public class ParticipantsMaxNumberReachedException : BusinessException
    {
        public ParticipantsMaxNumberReachedException(int maxParticipantsNumber)
            : base($"Numero massimo di partecipanti raggiunto ({maxParticipantsNumber}.")
        {
        }
    }
}
