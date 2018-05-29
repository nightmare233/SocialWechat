using Framework.WebApi;
using Microsoft.AspNet.SignalR;
using Social.Application.AppServices;
using Social.Application.Dto;
using Social.Infrastructure;
using Social.WebApi.Hubs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Social.WebApi.Controllers
{
    /// <summary>
    /// api used for push notification to front-end.
    /// </summary>
    [RoutePrefix("api/notifications")]
    public class NotificationController : ApiController
    {
        private Lazy<IHubContext> _hubLazy = new Lazy<IHubContext>(() => GlobalHost.ConnectionManager.GetHubContext<NotificationHub>());

        private IHubContext _hub
        {
            get { return _hubLazy.Value; }
        }

        private IConversationAppService _conversationAppService;
        private IConversationMessageAppService _messageAppService;
        private INotificationConnectionManager _notificationConnectionManager;


        /// <summary>
        /// NotificationController
        /// </summary>
        /// <param name="conversationAppService"></param>
        /// <param name="messageAppService"></param>
        /// <param name="notificationConnectionManager"></param>
        public NotificationController(
            IConversationAppService conversationAppService,
            IConversationMessageAppService messageAppService,
            INotificationConnectionManager notificationConnectionManager
            )
        {
            _conversationAppService = conversationAppService;
            _messageAppService = messageAppService;
            _notificationConnectionManager = notificationConnectionManager;
        }

        [Route("conversation-created")]
        [HttpGet]
        public IHttpActionResult ConversationCreated(int conversationId)
        {
            var dto = _conversationAppService.Find(conversationId);
            if (dto != null)
            {
                var connections = _notificationConnectionManager.GetConnections(Request.GetSiteId(), dto.AgentId, dto.DepartmentId);
                _hub.Clients.Clients(connections).conversationCreated(dto);
            }
            return Ok();
        }

        [Route("conversation-updated")]
        [HttpGet]
        public IHttpActionResult ConversationUpdated(int conversationId, int? oldMaxLogId = null)
        {
            var dto = _conversationAppService.Find(conversationId);
            if (dto != null)
            {
                _hub.Clients.Group(Request.GetSiteId().ToString()).conversationUpdated(dto);

                if (oldMaxLogId.HasValue && oldMaxLogId > 0)
                {
                    var dtoList = _conversationAppService.GetNewLogs(conversationId, oldMaxLogId.Value);
                    if (dtoList != null && dtoList.Any())
                    {
                        var connections = _notificationConnectionManager.GetConnections(Request.GetSiteId(), dto.AgentId, dto.DepartmentId);
                        _hub.Clients.Clients(connections).conversationLogCreated(dtoList);
                    }
                }
            }

            return Ok();
        }

        [Route("facebook-comment-created")]
        [HttpGet]
        public IHttpActionResult FacebookCommentMessageCreated(int messageId)
        {
            var message = _messageAppService.GetFacebookPostCommentMessage(messageId);
            if (message != null)
            {
                var conversation = _conversationAppService.Find(message.ConversationId);
                if (conversation != null)
                {
                    var connections = _notificationConnectionManager.GetConnections(Request.GetSiteId(), conversation.AgentId, conversation.DepartmentId);
                    var excludeConnections = _notificationConnectionManager.GetConnections(Request.GetSiteId(), message.SendAgentId.GetValueOrDefault());
                    _hub.Clients.Clients(connections.Except(excludeConnections).ToList()).facebookCommentCreated(message);
                    _hub.Clients.Clients(connections).conversationUpdated(conversation);
                }
            }
            return Ok();
        }

        [Route("facebook-message-created")]
        [HttpGet]
        public IHttpActionResult FacebookMessageCreated(int messageId)
        {
            var message = _messageAppService.GetFacebookDirectMessage(messageId);
            if (message != null)
            {
                var conversation = _conversationAppService.Find(message.ConversationId);
                if (conversation != null)
                {
                    var connections = _notificationConnectionManager.GetConnections(Request.GetSiteId(), conversation.AgentId, conversation.DepartmentId);
                    var excludeConnections = _notificationConnectionManager.GetConnections(Request.GetSiteId(), message.SendAgentId.GetValueOrDefault());
                    _hub.Clients.Clients(connections.Except(excludeConnections).ToList()).facebookMessageCreated(message);
                    _hub.Clients.Clients(connections).conversationUpdated(conversation);
                }
            }
            return Ok();
        }

        [Route("twitter-tweet-created")]
        [HttpGet]
        public IHttpActionResult TwitterTweetCreated(int messageId)
        {
            var message = _messageAppService.GetTwitterTweetMessage(messageId);
            if (message != null)
            {
                var conversation = _conversationAppService.Find(message.ConversationId);
                if (conversation != null)
                {
                    var connections = _notificationConnectionManager.GetConnections(Request.GetSiteId(), conversation.AgentId, conversation.DepartmentId);
                    var excludeConnections = _notificationConnectionManager.GetConnections(Request.GetSiteId(), message.SendAgentId.GetValueOrDefault());
                    _hub.Clients.Clients(connections.Except(excludeConnections).ToList()).twitterTweetCreated(message);
                    _hub.Clients.Clients(connections).conversationUpdated(conversation);
                }
            }
            return Ok();
        }

        [Route("twitter-direct-message-created")]
        [HttpGet]
        public IHttpActionResult TwitterDirectMessageCreated(int messageId)
        {
            var message = _messageAppService.GetTwitterDirectMessage(messageId);
            if (message != null)
            {
                var conversation = _conversationAppService.Find(message.ConversationId);
                if (conversation != null)
                {
                    var connections = _notificationConnectionManager.GetConnections(Request.GetSiteId(), conversation.AgentId, conversation.DepartmentId);
                    var excludeConnections = _notificationConnectionManager.GetConnections(Request.GetSiteId(), message.SendAgentId.GetValueOrDefault());
                    _hub.Clients.Clients(connections.Except(excludeConnections).ToList()).twitterDirectMessageCreated(message);
                    _hub.Clients.Clients(connections).conversationUpdated(conversation);
                }
            }
            return Ok();
        }


        [Route("public-filter-created")]
        [HttpGet]
        public IHttpActionResult PublicFilterCreated(int filterId)
        {
            _hub.Clients.Group(Request.GetSiteId().ToString()).publicFilterCreated(filterId);
            return Ok();
        }

        [Route("public-filter-deleted")]
        [HttpGet]
        public IHttpActionResult PublicFilterDeleted(int filterId)
        {
            _hub.Clients.Group(Request.GetSiteId().ToString()).publicFilterDeleted(filterId);
            return Ok();
        }

        [Route("public-filter-updated")]
        [HttpGet]
        public IHttpActionResult PublicFilterUpdated(int filterId)
        {
            _hub.Clients.Group(Request.GetSiteId().ToString()).publicFilterUpdated(filterId);
            return Ok();
        }
    }
}