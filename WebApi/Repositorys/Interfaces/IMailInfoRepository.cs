using PersistenceNet.Structs;
using WebApi.Entitys;

namespace WebApi.Repositorys.Interfaces
{
    public interface IMailInfoRepository
    {
        Task<MailInfo> Get(string organizationCode);
        Task<OperationReturn> New(MailInfo mailInfo);
        Task<OperationReturn> Update(MailInfo mailInfo);
    }
}