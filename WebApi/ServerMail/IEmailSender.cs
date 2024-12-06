namespace WebApi.ServerMail
{
    public interface IEmailSender
    {
        Task SenderEmailAsync(string from, string to, string toName
            , string subject, string message, bool userSsl = false);
    }
}