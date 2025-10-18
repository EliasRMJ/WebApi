using Microsoft.EntityFrameworkCore;
using PersistenceNet;
using PersistenceNet.Structs;
using WebApi.ContextDB;
using WebApi.Entitys;
using WebApi.Repositorys.Interfaces;

namespace WebApi.Repositorys
{
    internal class MailInfoRepository(WebApiContext sysContext) 
        : PersistenceData<MailInfo>(sysContext), IMailInfoRepository
    {
        async public Task<OperationReturn> New(MailInfo mailInfo) => await NewAsync(mailInfo);

        async public Task<OperationReturn> Update(MailInfo mailInfo) => await UpdateAsync(mailInfo);

        async public Task<MailInfo> Get(string organizationCode)
        {
#pragma warning disable CS8603
            return await sysContext.MailInfos
                   .AsNoTrackingWithIdentityResolution()
                   .FirstOrDefaultAsync(rt => rt.OrganizationCode == organizationCode && 
                                              rt.Active == PersistenceNet.Enuns.ActiveEnum.S);
#pragma warning restore CS8603 
        }
    }
}