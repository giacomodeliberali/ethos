using System.ComponentModel.DataAnnotations;

namespace Ethos.Application.Contracts
{
    /// <summary>
    /// Every exception will be serialized to the client wrapped in this class.
    /// </summary>
    [TypeScript]
    public class ExceptionDto
    {
        [Required]
        public string Message { get; set; }
    }
}
