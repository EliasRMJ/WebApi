using PersistenceNet.Structs;
using WebApi.ViewModels;

namespace WebApi.Managers.Interfaces
{
    public interface IMailInfoManager
    {
        Task<MailInfoViewModel?> Get(string organizationCode);
        Task<OperationReturn> NewOrReplace(MailInfoViewModel mailInfoViewModel);
    }
}