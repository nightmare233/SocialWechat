using Framework.Core;
using Framework.Core.EntityFramework;
using Social.Domain.Core;
using Social.Domain.Entities;

namespace Social.Domain.Repositories
{
    public interface ISiteSocialAccountRepository : IRepository<GeneralDataContext, SiteSocialAccount, int>
    {
    }

    public class SiteSocialAccountRepository : EfRepository<GeneralDataContext, SiteSocialAccount, int>, ISiteSocialAccountRepository
    {
    }
}
