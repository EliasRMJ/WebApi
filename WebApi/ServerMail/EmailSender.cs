using MailKit.Security;
using MimeKit;
using PersistenceNet.Enuns;
using Quiron.Mail;
using System.Text;
using WebApi.ViewModels;

namespace WebApi.ServerMail
{
    public class EmailSender: ServerEmail, IEmailSender
    {
        private MailInfoViewModel? _mailInfo = null;

        protected override string ContainerHtml(string body)
        {
            if (this._mailInfo is null)
                return body;

            var sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html lang='pt-BR'>");
            sb.AppendLine("<head>");
            sb.AppendLine("  <meta charset='UTF-8'>");
            sb.AppendLine("  <meta name='viewport' content='width=device-width, initial-scale=1.0'>");
            sb.AppendFormat("<title>{0}</title>", this._mailInfo.CompanyName);
            sb.AppendLine("  <style>");
            sb.AppendLine("      .container {");
            sb.AppendLine("        max-width: 600px;");
            sb.AppendLine("        margin: 20px auto;");
            sb.AppendLine("        background-color: #ffffff;");
            sb.AppendLine("        padding: 20px;");
            sb.AppendLine("        border-radius: 12px;");
            sb.AppendLine("        box-shadow: 0 2px 6px rgba(0, 0, 0, 0.05);");
            sb.AppendLine("        text-align: center;");
            sb.AppendLine("      }");
            sb.AppendLine("      .header img {");
            sb.AppendLine("        max-width: 100%;");
            sb.AppendLine("        height: auto;");
            sb.AppendLine("        display: block;");
            sb.AppendLine("        margin: 0 auto 20px;");
            sb.AppendLine("        border-radius: 8px;");
            sb.AppendLine("      }");
            sb.AppendLine("      .content {");
            sb.AppendLine("        font-size: 16px;");
            sb.AppendLine("        color: #333333;");
            sb.AppendLine("        line-height: 1.5;");
            sb.AppendLine("      }");
            sb.AppendLine("      .button-wrapper {");
            sb.AppendLine("        text-align: center;");
            sb.AppendLine("        margin: 30px 0;");
            sb.AppendLine("      }");
            sb.AppendLine("      .button {");
            sb.AppendLine("        display: inline-block;");
            sb.AppendLine("        padding: 12px 24px;");
            sb.AppendLine("        background-color: #4CAF50;");
            sb.AppendLine("        color: #ffffff;");
            sb.AppendLine("        text-decoration: none;");
            sb.AppendLine("        font-size: 16px;");
            sb.AppendLine("        font-weight: bold;");
            sb.AppendLine("        border-radius: 6px;");
            sb.AppendLine("        transition: background-color 0.3s ease;");
            sb.AppendLine("      }");
            sb.AppendLine("      .button:hover {");
            sb.AppendLine("        background-color: #45a049;");
            sb.AppendLine("      }");
            sb.AppendLine("      .text-body {");
            sb.AppendLine("        text-align: center;");
            sb.AppendLine("        font-size: 14px;");
            sb.AppendLine("        color: #575858;");
            sb.AppendLine("      }");
            sb.AppendLine("      .text-label {");
            sb.AppendLine("        text-align: center;");
            sb.AppendLine("        font-size: 12px;");
            sb.AppendLine("        color: #999999;");
            sb.AppendLine("      }");
            sb.AppendLine("      .footer {");
            sb.AppendLine("        text-align: center;");
            sb.AppendLine("        font-size: 12px;");
            sb.AppendLine("        color: #999999;");
            sb.AppendLine("        margin-top: 30px;");
            sb.AppendLine("        padding-top: 10px;");
            sb.AppendLine("        border-top: 1px solid #eeeeee;");
            sb.AppendLine("      }");
            sb.AppendLine("      @media only screen and (max-width: 600px) {");
            sb.AppendLine("        .container {");
            sb.AppendLine("          padding: 15px;");
            sb.AppendLine("          margin: 10px; ");
            sb.AppendLine("        }");
            sb.AppendLine("      }");
            sb.AppendLine("  </style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body style='margin: 0;padding: 10px;background-color: #f6f6f6;font-family: Arial, sans-serif;'>");
            sb.AppendLine("  <div class='container'>");
            sb.AppendLine("    <div class='header'>");
            sb.AppendFormat("      <img src='{0}' width='80' />", this._mailInfo.UrlLogo);
            sb.AppendLine("    </div>");
            sb.AppendLine("    <div class='content'>");
            sb.AppendFormat("     {0}", body);
            sb.AppendLine("    </div>");
            sb.AppendLine("    <div class='footer'>");
            sb.AppendLine("       <img src='https://web.servicenow.app.br/img/logo.png' width='35' alt='ServiceNow' /><br>");
            sb.AppendFormat("     &copy; <span id='year'>{0}</span> ServiceNow. Todos os direitos reservados", DateTime.UtcNow.Year);
            sb.AppendLine("    </div>");
            sb.AppendLine("  </div>");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            return sb.ToString();
        }

        public async Task SenderEmailAsync(MailInfoViewModel mailInfo, string from, string to, string toName
            , string subject, string message, MailAttachment[] mailAttachments
            , MessagePriority messagePriority = MessagePriority.Normal)
        {
            this._mailInfo = mailInfo;

            base.Host = mailInfo.Server;
            base.UserMail = mailInfo.AccountMail;
            base.Password = mailInfo.Password;
            base.Port = mailInfo.Port;
            base.UserSsl = (mailInfo.SecureConnection.Equals(ActiveEnum.S));
            base.SecureSocketOptions = SecureSocketOptions.StartTls;

            await base.SendMailAsync(new ParamEmail(from, from), new ParamEmail(toName, to), subject
                    , message, mailAttachments, messagePriority);
        }
    }
}