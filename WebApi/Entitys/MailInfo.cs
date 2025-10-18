using PersistenceNet.Enuns;
using PersistenceNet.Interfaces;
using PersistenceNet.Views;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using WebApi.ViewModels;

namespace WebApi.Entitys
{
    [DebuggerDisplay("{AccountMail}")]
    [DisplayName("Mail Info")]
    [Table("web_server_mail_info")]
    public class MailInfo : IElement
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column("web_id", Order = 1)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Informe o e-mail para autenticação!")]
        [MaxLength(70, ErrorMessage = "E-mail não pode conter mais de 70 caracteres!")]
        [Column("web_account_mail", Order = 2)]
        public string? AccountMail { get; set; }

        [Required(ErrorMessage = "Informe 'S' ou 'N' para controle de visualização!")]
        [MaxLength(1, ErrorMessage = "Ativo de ser 'S' ou 'N' e não pode conter mais de 1 caracter!")]
        [Column("web_active", Order = 3)]
        public ActiveEnum Active { get; set; }

        [Required(ErrorMessage = "Informe a senha para autenticação!")]
        [MaxLength(45, ErrorMessage = "A senha da autenticação não pode conter mais de 400 caracteres!")]
        [Column("web_pwd", Order = 4)]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Informe a porta!")]
        [Column("web_port", Order = 5)]
        public int Port { get; set; }

        [Required(ErrorMessage = "Informe 'S' ou 'N' para controle de visualização!")]
        [MaxLength(1, ErrorMessage = "Ativo de ser 'S' ou 'N' e não pode conter mais de 1 caracter!")]
        [Column("web_secure_connection", Order = 6)]
        public ActiveEnum SecureConnection { get; set; }

        [Required(ErrorMessage = "Informe o servidor para autenticação!")]
        [MaxLength(70, ErrorMessage = "O servidor da autenticação não pode conter mais de 400 caracteres!")]
        [Column("web_server", Order = 7)]
        public string? Server { get; set; }

        [Required(ErrorMessage = "Informe o código da organização para autenticação!")]
        [MaxLength(100, ErrorMessage = "O código da organização da autenticação não pode conter mais de 400 caracteres!")]
        [Column("web_organization_code", Order = 8)]
        public string? OrganizationCode { get; set; }

        [MaxLength(200, ErrorMessage = "A url do logo não pode conter mais de 200 caracteres!")]
        [Column("web_url_logo", Order = 9)]
        public string? UrlLogo { get; set; }

        [Required(ErrorMessage = "Informe o nome da empresa!")]
        [MaxLength(70, ErrorMessage = "O npme da empresa não pode conter mais de 70 caracteres!")]
        [Column("web_company_name", Order = 10)]
        public string? CompanyName { get; set; }

        [NotMapped]
        public ElementStatesEnum ElementStates { get => Id.Equals(0) ? ElementStatesEnum.New : ElementStatesEnum.Update; set { } }

        public ViewBase GetView()
        {
            return new MailInfoViewModel
            {
                AccountMail = this.AccountMail,
                OrganizationCode = this.OrganizationCode,
                Password = this.Password,
                Port = this.Port,
                Server = this.Server,
                UrlLogo = this.UrlLogo,
                CompanyName = this.CompanyName,
                SecureConnection = this.SecureConnection,
                IsActive = Active == ActiveEnum.S,
                IsCheck = false,
                IsDelete = false,
                Id = Id,
            };
        }
    }
}