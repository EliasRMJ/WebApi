using PersistenceNet.Structs;
using WebApi.ContextDB;
using WebApi.Entitys;
using WebApi.Managers.Interfaces;
using WebApi.Repositorys;
using WebApi.Repositorys.Interfaces;
using WebApi.ViewModels;

namespace WebApi.Managers
{
    public sealed class MailInfoManager : Manager, IMailInfoManager
    {
        private readonly IMailInfoRepository _mailInfoRepository;

        public MailInfoManager(WebApiContext sGSContext)
        {
            base._sGSContext = sGSContext;
            _mailInfoRepository = new MailInfoRepository(sGSContext);
        }

        public MailInfoManager(Manager manager) 
            : base(manager)
        {
            base._sGSContext = manager._sGSContext;
            _mailInfoRepository = new MailInfoRepository(manager._sGSContext);
        }

        public async Task<MailInfoViewModel?> Get(string organizationCode)
        {
            var mailInfoExist = await _mailInfoRepository.Get(organizationCode);
            return mailInfoExist?.GetView() as MailInfoViewModel;
        }

        public async Task<OperationReturn> NewOrReplace(MailInfoViewModel mailInfoViewModel)
        {
            var reasonExist = await _mailInfoRepository.Get(mailInfoViewModel.OrganizationCode!);

            return reasonExist is null ? 
                await _mailInfoRepository.New(mailInfoViewModel.ConvertTo() as MailInfo) 
              : await _mailInfoRepository.Update(mailInfoViewModel.ConvertTo(reasonExist) as MailInfo);
        }
    }
}