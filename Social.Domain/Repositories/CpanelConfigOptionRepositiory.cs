using Framework.Core;
using Framework.Core.EntityFramework;
using Framework.Core.UnitOfWork;
using Social.Domain.Core;
using Social.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Social.Domain.Repositories
{
    public interface ICpanelConfigOptionRepositiory
    {
        string LiveChatCacheMagicString { get; }
        string LiveChatServerChatHost { get; }
    }
    public class CpanelConfigOptionRepositiory : EfRepository<GeneralDataContext,CpanelConfigOption,string>,ICpanelConfigOptionRepositiory
    {
        public string LiveChatCacheMagicString => FindConfigValue("LiveChat_CacheMagicString");
        public string LiveChatServerChatHost => FindConfigValue("LiveChat_ServerChatHost");


        private string FindConfigValue(string configKey)
        {
            using (var uow = UnitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                using (CurrentUnitOfWork.UseGeneralDB())
                {
                    var config = Find(configKey);
                    uow.Complete();
                    return config?.OptionValue;
                }
            }
        }
    }
}
