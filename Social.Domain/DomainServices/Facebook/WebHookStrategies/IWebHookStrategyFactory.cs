using Framework.Core;
using Social.Infrastructure.Facebook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Domain.DomainServices.Facebook
{
    public interface IConversationStrategyFactory : ITransient
    {
        IWebHookSrategy Create(FbHookChange change);
    }
}
