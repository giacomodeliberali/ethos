using System;

namespace Ethos.Domain
{
    public class BusinessException : Exception
    {
        public BusinessException(string message, Exception innerException = null)
            : base(message, innerException)
        {
        }
    }
}
