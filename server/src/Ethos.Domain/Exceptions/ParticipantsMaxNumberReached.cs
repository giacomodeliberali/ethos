namespace Ethos.Domain.Exceptions
{
    public class ParticipantsMaxNumberReached : BusinessException
    {
        public ParticipantsMaxNumberReached(int maxParticipantsNumber)
            : base($"Numero massimo di partecipanti raggiunto ({maxParticipantsNumber}.")
        {
        }
    }
}
