using Framework.Core;
using Social.Domain.DomainServices;
using Social.Domain.DomainServices.Facebook;
using Social.Domain.Entities;
using Social.Infrastructure.Facebook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Application.AppServices
{
    public interface IFacebookAppService
    {
        Task ProcessWebHookData(FbHookData fbData);
        Task PullTaggedVisitorPosts(SocialAccount socialAccount);
        Task PullVisitorPostsFromFeed(SocialAccount socialAccount);
        Task PullMessagesJob(SocialAccount socialAccount);
    }

    public class FacebookAppService : AppService, IFacebookAppService
    {
        private IWebHookService _facebookWebHookService;
        private IPullJobService _facebookPullJobService;

        public FacebookAppService(
            IWebHookService facebookWebHookService,
            IPullJobService facebookPullJobService
            )
        {
            _facebookWebHookService = facebookWebHookService;
            _facebookPullJobService = facebookPullJobService;
        }

        public async Task ProcessWebHookData(FbHookData fbData)
        {
            await _facebookWebHookService.ProcessWebHookData(fbData);
        }

        public async Task PullMessagesJob(SocialAccount socialAccount)
        {
            await _facebookPullJobService.PullMessagesJob(socialAccount);
        }

        public async Task PullTaggedVisitorPosts(SocialAccount socialAccount)
        {
            await _facebookPullJobService.PullTaggedVisitorPosts(socialAccount);
        }

        public async Task PullVisitorPostsFromFeed(SocialAccount socialAccount)
        {
            await _facebookPullJobService.PullVisitorPostsFromFeed(socialAccount);
        }
    }
}
