using PersistenceNet.Enuns;
using PersistenceNet.Views;
using Newtonsoft.Json;
using WebApi.Entitys;

namespace WebApi.ViewModels
{
    public class MailInfoViewModel: ViewBase
    {
        [JsonIgnore]
        public override string ClassName => "Servidor E-mail";

        public string? AccountMail { get; set; }
        public ActiveEnum Active { get; set; }
        public string? Password { get; set; }
        public int Port { get; set; }
        public ActiveEnum SecureConnection { get; set; }
        public string? Server { get; set; }
        public string? UrlLogo { get; set; }
        public string? CompanyName { get; set; }

        public override object? ConvertTo(object? gDomain = null)
        {
            var mailInfo = (gDomain == null) ? new MailInfo { Id = this.Id } : (MailInfo)gDomain;

            mailInfo.AccountMail = this.AccountMail;
            mailInfo.SecureConnection = this.SecureConnection;
            mailInfo.Password = this.Password;
            mailInfo.Active = this.IsActive ? ActiveEnum.S : ActiveEnum.N;
            mailInfo.Port = this.Port;
            mailInfo.OrganizationCode = this.OrganizationCode;
            mailInfo.Server = this.Server;
            mailInfo.UrlLogo = this.UrlLogo;
            mailInfo.CompanyName = this.CompanyName;

            return mailInfo;
        }
    }
}