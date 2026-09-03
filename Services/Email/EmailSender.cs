using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Threading.Tasks;

namespace Quantify.Services.Email;

public class EmailSender : IEmailSender
{
    private readonly EmailSettings _emailSettings;

    public EmailSender(IOptions<EmailSettings> emailSettings)
    {
        _emailSettings = emailSettings.Value;
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var emailMessage = new MimeMessage();

        // Ustawienie nadawcy (np. Quantify Platform <twojmail@gmail.com>)
        emailMessage.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
        
        // Ustawienie odbiorcy
        emailMessage.To.Add(new MailboxAddress("", email));
        
        emailMessage.Subject = subject;

        // Budowanie treści maila jako HTML
        var bodyBuilder = new BodyBuilder { HtmlBody = htmlMessage };
        emailMessage.Body = bodyBuilder.ToMessageBody();

        // Konfiguracja klienta SMTP z MailKit
        using (var client = new SmtpClient())
        {
            // Workaround for macOS CRL check issue with Google SMTP
            client.CheckCertificateRevocation = false;

            // Łączenie z serwerem (np. gmail) z użyciem bezpiecznego protokołu TLS
            await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, SecureSocketOptions.Auto);
            
            // Logowanie za pomocą Twojego App Password
            await client.AuthenticateAsync(_emailSettings.SenderEmail, _emailSettings.Password);
            
            // Właściwa wysyłka
            await client.SendAsync(emailMessage);
            
            // Rozłączenie
            await client.DisconnectAsync(true);
        }
    }
}
