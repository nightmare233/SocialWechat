using Framework.Core;
using Social.Domain.Entities;
using Social.Infrastructure;
using Social.Infrastructure.Twitter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi.Credentials.Models;
using Tweetinvi.Models;

namespace Social.Domain.DomainServices
{
    public interface ITwitterAuthService : IDomainService<TwitterAuth>
    {
        IAuthenticationContext InitAuthentication(string redirectUri);
        Task<IAuthenticatedUser> ValidateAuthAsync(string authorizationId, string oauthVerifier);
    }

    public class TwitterAuthService : DomainService<TwitterAuth>, ITwitterAuthService
    {
        public ITwitterClient _twitterClient;

        public TwitterAuthService(ITwitterClient twitterClient)
        {
            _twitterClient = twitterClient;
        }

        private async Task<TwitterAuth> FindAndDeleteAsync(string authorizationId)
        {
            var auth = Repository.FindAll().Where(t => t.AuthorizationId == authorizationId).FirstOrDefault();

            if (auth != null)
            {
                await UnitOfWorkManager.RunWithNewTransaction(CurrentUnitOfWork.GetSiteId(), async () =>
                 {
                     await Repository.DeleteAsync(auth.Id);
                 });
            }

            return auth;
        }

        public IAuthenticationContext InitAuthentication(string redirectUri)
        {
            IAuthenticationContext authenticationContext = _twitterClient.InitAuthentication(redirectUri);

            Repository.Insert(new TwitterAuth
            {
                AuthorizationId = authenticationContext.Token.AuthorizationUniqueIdentifier,
                AuthorizationKey = authenticationContext.Token.AuthorizationKey,
                AuthorizationSecret = authenticationContext.Token.AuthorizationSecret
            });

            return authenticationContext;
        }

        public async Task<IAuthenticatedUser> ValidateAuthAsync(string authorizationId, string oauthVerifier)
        {
            var twitterAuth = await FindAndDeleteAsync(authorizationId);

            if (string.IsNullOrEmpty(oauthVerifier))
            {
                return null;
            }

            return _twitterClient.GetAuthenticatedUser(oauthVerifier, twitterAuth.AuthorizationKey, twitterAuth.AuthorizationSecret);
        }
    }
}
