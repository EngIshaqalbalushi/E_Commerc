using E_CommerceSystem.Services;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

public class EmailService : IEmailService
{
    private readonly SmtpSettings _settings;

    public EmailService(IOptions<SmtpSettings> settings)
    {
        _settings = settings.Value;
    }

    public void SendEmail(string to, string subject, string body)
    {
        using (var client = new SmtpClient(_settings.Server, _settings.Port))
        {
            client.Credentials = new NetworkCredential(_settings.User, _settings.Pass);
            client.EnableSsl = true;

            var mail = new MailMessage();
            mail.From = new MailAddress(_settings.User, "E-Commerce System");
            mail.To.Add(to);
            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = true;

            client.Send(mail);
        }
    }
}
