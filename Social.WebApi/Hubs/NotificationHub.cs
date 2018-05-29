using Framework.Core.SignalR;
using Microsoft.AspNet.SignalR;
using Social.Infrastructure;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Social.WebApi.Hubs
{
    [Authorize]
    public class NotificationHub : Hub
    {
        public override Task OnConnected()
        {
            var connectionManager = GetConnectionManager();
            int userId = Context.Request.User.Identity.GetUserId().GetValueOrDefault();
            int siteId = Context.Request.User.Identity.GetSiteId().GetValueOrDefault();
            connectionManager.Connect(siteId, userId, Context.ConnectionId);
            Groups.Add(Context.ConnectionId, GetSiteId());
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            var connectionManager = GetConnectionManager();
            int userId = Context.Request.User.Identity.GetUserId().GetValueOrDefault();
            int siteId = Context.Request.User.Identity.GetSiteId().GetValueOrDefault();
            connectionManager.Disconnect(siteId, userId, Context.ConnectionId);
            Groups.Remove(Context.ConnectionId, GetSiteId());
            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {
            var connectionManager = GetConnectionManager();
            int userId = Context.Request.User.Identity.GetUserId().GetValueOrDefault();
            int siteId = Context.Request.User.Identity.GetSiteId().GetValueOrDefault();
            connectionManager.Reconnect(siteId, userId, Context.ConnectionId);
            Groups.Add(Context.ConnectionId, GetSiteId());
            return base.OnReconnected();
        }

        private string GetSiteId()
        {
            int siteId = Context.Request.User.Identity.GetSiteId().GetValueOrDefault();
            return siteId.ToString();
        }

        private string GetUserId()
        {
            int userId = Context.Request.User.Identity.GetUserId().GetValueOrDefault();
            return userId.ToString();
        }

        public INotificationConnectionManager GetConnectionManager()
        {
            return GlobalHost.DependencyResolver.GetService(typeof(INotificationConnectionManager)) as INotificationConnectionManager;
        }
    }
}