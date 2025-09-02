using MailKit.Net.Smtp;
using MimeKit;

namespace E_CommerceSystem.Services
{
    public interface IEmailService
    {
        void SendEmail(string toEmail, string subject, string body);
    }

    public class EmailService : IEmailService
    {
        private readonly string _fromEmail = "youremail@gmail.com"; // replace
        private readonly string _password = "your-app-password";   // replace (use App Password)

        public void SendEmail(string toEmail, string subject, string body)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("E-Commerce System", _fromEmail));
            email.To.Add(new MailboxAddress("", toEmail));
            email.Subject = subject;

            email.Body = new TextPart("html")
            {
                Text = body
            };

            using (var smtp = new SmtpClient())
            {
                smtp.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                smtp.Authenticate(_fromEmail, _password);
                smtp.Send(email);
                smtp.Disconnect(true);
            }
        }
    }
}
