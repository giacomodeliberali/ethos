namespace Ethos.Domain.Exceptions
{
    public class AlreadyBookedException : BusinessException
    {
        public AlreadyBookedException()
            : base("Hai già prenotato questo corso")
        {
        }
    }
}
