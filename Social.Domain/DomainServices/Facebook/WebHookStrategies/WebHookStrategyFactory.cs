using Framework.Core;
using Social.Infrastructure.Facebook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Domain.DomainServices.Facebook
{
    public class WebHookStrategyFactory : IConversationStrategyFactory
    {
        private IDependencyResolver _dependencyResolver;

        public WebHookStrategyFactory(IDependencyResolver dependencyResolver)
        {
            _dependencyResolver = dependencyResolver;
        }

        public IWebHookSrategy Create(FbHookChange change)
        {
            var strategies = _dependencyResolver.ResolveAll<IWebHookSrategy>();
            var strategory = strategies.FirstOrDefault(t => t.IsMatch(change));
            return strategory;
        }
    }
}
