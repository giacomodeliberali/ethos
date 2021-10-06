using System;

namespace Ethos.Domain.Exceptions
{
    public class AuthenticationException : BusinessException
    {
        public AuthenticationException(string message)
            : base(message)
        {
        }
    }
}
