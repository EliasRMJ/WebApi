using MimeKit;

namespace WebApi.ServerMail
{
    public class EmailSender(IConfiguration configuration) : IEmailSender
    {
        readonly IConfiguration _configuration = configuration;

        public async Task SenderEmailAsync(string from, string to, string toName
            , string subject, string message, bool userSsl = false)
        {
            var mimeMessage = new MimeMessage();
            mimeMessage.From.Add(new MailboxAddress(from, from));
            mimeMessage.To.Add(new MailboxAddress(toName, to));
            mimeMessage.Subject = subject;
            mimeMessage.Body = new TextPart("html") { Text = message };
            mimeMessage.Priority = MessagePriority.Normal;

            using var client = new MailKit.Net.Smtp.SmtpClient();
            client.ServerCertificateValidationCallback = (s, c, h, e) => true;
            client.Connect(_configuration["SMTP:Host"], int.Parse(_configuration["SMTP:Port"]!), userSsl);
            client.Authenticate(_configuration["SMTP:Username"], _configuration["SMTP:Password"]);

            try
            {
                await client.SendAsync(mimeMessage);
            }
            catch (Exception ex)
            {
                throw new Exception("ERROR: Ocorreu uma exceção ao enviar email. Ref.: MAIL"
                    , ex);
            }
            finally
            {
                await client.DisconnectAsync(true);
            }
        }
    }
}