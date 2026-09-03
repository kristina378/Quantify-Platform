using System.Threading.Tasks;

namespace Quantify.Services.Email;

public interface IEmailSender
{
    Task SendEmailAsync(string email, string subject, string htmlMessage);
}