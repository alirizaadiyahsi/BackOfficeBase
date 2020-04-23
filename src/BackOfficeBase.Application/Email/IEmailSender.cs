using System.Threading.Tasks;

namespace BackOfficeBase.Application.Email
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}