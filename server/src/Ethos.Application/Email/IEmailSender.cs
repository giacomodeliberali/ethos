using System.Threading.Tasks;

namespace Ethos.Application.Email
{
    public interface IEmailSender
    {
        Task SendEmail(string recipient, string subject, string message);
    }
}
