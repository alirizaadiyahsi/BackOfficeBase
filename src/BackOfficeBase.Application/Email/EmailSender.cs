using System;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace BackOfficeBase.Application.Email
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger _logger;

        public EmailSender(
            IOptions<EmailSettings> emailSettings, ILogger logger)
        {
            _logger = logger;
            _emailSettings = emailSettings.Value;
        }

        // TODO: Write test
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            try
            {
                var mimeMessage = new MimeMessage();
                mimeMessage.From.Add(new MailboxAddress(_emailSettings.SenderUsername, _emailSettings.SenderEmail));
                mimeMessage.To.Add(new MailboxAddress(email));
                mimeMessage.Subject = subject;
                mimeMessage.Body = new TextPart("html")
                {
                    Text = message
                };

#if DEBUG
                _logger.LogInformation(@$"-------- Send Email: From: {_emailSettings.SenderEmail} - To: {email}
                                       \n -------- Subject: {subject}
                                       \n -------- Body: {message}");
#else
                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(_emailSettings.Host, _emailSettings.Port, _emailSettings.EnableSsl);
                    await client.AuthenticateAsync(_emailSettings.SenderEmail, _emailSettings.Password);
                    await client.SendAsync(mimeMessage);
                    await client.DisconnectAsync(true);
                }
#endif



            }
            catch (Exception ex)
            {
                // TODO: handle exception
                throw new InvalidOperationException(ex.Message);
            }
        }
    }
}