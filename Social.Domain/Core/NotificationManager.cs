using Social.Domain;
using Social.Infrastructure.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;

namespace Social.Infrastructure.Core
{
    public class NotificationManager : INotificationManager
    {
        private INotificationConnectionManager _connectionManager;
        private IFilterService _filterService;

        public NotificationManager(
            INotificationConnectionManager connectionManager,
            IFilterService filterService
            )
        {
            _connectionManager = connectionManager;
            _filterService = filterService;
        }

        private TimeSpan _delayTimeSpan = TimeSpan.Zero;

        public async Task NotifyNewConversation(int siteId, int conversationId)
        {
            await GetAsync($"/conversation-created?siteId={siteId}&conversationId={conversationId}");
        }

        public async Task NotifyUpdateConversation(int siteId, int conversationId, int? oldMaxLogId)
        {
            string url = $"/conversation-updated?siteId={siteId}&conversationId={conversationId}";
            if (oldMaxLogId != null && oldMaxLogId > 0)
            {
                url += $"&oldMaxLogId={oldMaxLogId}";
            }

            await GetAsync(url);
        }

        public async Task NotifyNewFacebookComment(int siteId, int messageId)
        {
            await GetAsync($"/facebook-comment-created?siteId={siteId}&messageId={messageId}");
        }

        public async Task NotifyNewFacebookMessage(int siteId, int messageId)
        {
            await GetAsync($"/facebook-message-created?siteId={siteId}&messageId={messageId}");
        }

        public async Task NotifyNewTwitterTweet(int siteId, int messageId)
        {
            await GetAsync($"/twitter-tweet-created?siteId={siteId}&messageId={messageId}");
        }

        public async Task NotifyNewTwitterDirectMessage(int siteId, int messageId)
        {
            await GetAsync($"/twitter-direct-message-created?siteId={siteId}&messageId={messageId}");
        }

        public async Task NotifyNewFilter(int siteId, int filterId)
        {
            //var filter = _filterService.FindFilterInlucdeConditions(filterId);
            //_connectionManager.RefreshCacheItem(filter, OperationType.Add);
            //if (filter.IfPublic)
            //{
            await GetAsync($"/public-filter-created?siteId={siteId}&filterId={filterId}");
            //}
        }

        public async Task NotifyDeleteFilter(int siteId, int filterId)
        {
            //var filter = _filterService.FindFilterInlucdeConditions(filterId);
            //_connectionManager.RefreshCacheItem(filter, OperationType.Delete);
            //if (filter.IfPublic)
            //{
            await GetAsync($"/public-filter-deleted?siteId={siteId}&filterId={filterId}");
            //}
        }

        public async Task NotifyUpdateFilter(int siteId, int filterId)
        {
            //var filter = _filterService.FindFilterInlucdeConditions(filterId);
            //_connectionManager.RefreshCacheItem(filter, OperationType.Update);
            //if (filter.IfPublic)
            //{
            await GetAsync($"/public-filter-updated?siteId={siteId}&filterId={filterId}");
            //}
        }

        public async Task GetAsync(string url)
        {
            HttpClient client = new HttpClient();
            url = AppSettings.NotificationApiBaseAddress + url;
            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                string msg = await response.Content.ReadAsStringAsync();
                Logger.Error(msg, null);
            }
        }

        public async Task PostAsync<T>(string url, T value)
        {
            HttpClient client = new HttpClient();
            url = AppSettings.NotificationApiBaseAddress + url;
            var response = await client.PostAsync(url, value, new JsonMediaTypeFormatter());
            if (!response.IsSuccessStatusCode)
            {
                string msg = await response.Content.ReadAsStringAsync();
                Logger.Error(msg, null);
            }
        }
    }
}
