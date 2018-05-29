using Framework.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Social.Infrastructure.Facebook
{
    public interface IFbClient : ITransient
    {
        Task<FbPagingData<T>> GetPagingData<T>(string url);
        Task<FbToken> GetApplicationToken();
        string GetAuthUrl(string redirectUri);
        FbComment GetComment(string token, string fbCommentId);
        Task<FbPagingData<FbConversation>> GetConversationsMessages(string pageId, string token);
        FbUser GetFacebookUserInfo(string token, string fbUserId);
        Task<FbUser> GetMe(string token);
        Task<IList<FbMessage>> GetMessagesFromConversationId(string token, string fbConversationId, int limit);
        Task<IList<FbPage>> GetPages(string userToken);
        Task<FbPost> GetPost(string token, string fbPostId);
        Task<FbComment> GetPostComment(string commentId, string token, int limit = 100);
        Task<FbPagingData<FbPost>> GetTaggedVisitorPosts(string pageId, string token);
        Task<FbUser> GetUserInfo(string fbUserId);
        Task<FbUser> GetUserInfo(string token, string fbUserId);
        string GetUserToken(string code, string redirectUri);
        Task<FbPagingData<FbPost>> GetVisitorPosts(string pageId, string token);
        Task Process<T>(int siteId, string token, FbPagingData<T> fbPagingData, Func<int, string, T, Task> processFunc);
        string PublishComment(string token, string parentId, string message);
        string PublishMessage(string token, string fbConversationId, string message);
        string PublishPost(string pageId, string token, string message);
        Task SubscribeApp(string pageId, string pageToken);
        Task UnSubscribeApp(string pageId, string pageToken);
    }
}