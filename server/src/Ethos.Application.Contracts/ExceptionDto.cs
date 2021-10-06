using System;
using System.ComponentModel.DataAnnotations;

namespace Ethos.Application.Contracts
{
    [TypeScript]
    public class ExceptionDto
    {
        [Required]
        public string Message { get; set; }

        public string StackTrace { get; set; }

        public ExceptionDto InnerException { get; set; }
    }
}
