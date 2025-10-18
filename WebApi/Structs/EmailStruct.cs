using MimeKit;
using Quiron.Mail;

namespace WebApi.Structs
{
    public record struct EmailStruct(
          string From
        , string To
        , string ToName
        , string Subject
        , string Message
        , bool UserSsl
        , MailAttachment[] MailAttachments
        , MessagePriority MessagePriority = MessagePriority.Normal);
}