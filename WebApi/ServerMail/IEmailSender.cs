using MimeKit;
using Quiron.Mail;
using WebApi.ViewModels;

namespace WebApi.ServerMail
{
    public interface IEmailSender
    {
        Task SenderEmailAsync(MailInfoViewModel mailInfo, string from, string to, string toName
            , string subject, string message, MailAttachment[] mailAttachments
            , MessagePriority messagePriority = MessagePriority.Normal);
    }
}