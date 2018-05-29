using Framework.Core;
using Framework.Core.SignalR;
using Social.Domain.DomainServices;
using Social.Domain.Entities;
using Social.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Social.Infrastructure.Enum;
using LinqKit;
using AutoMapper;
using Social.Infrastructure.Cache;

namespace Social.Domain.Core
{
    public class NotificationConnectionManager : ServiceBase, INotificationConnectionManager
    {
        private readonly static ConnectionMapping<AgentCacheItem> _agentConnectionCache = new ConnectionMapping<AgentCacheItem>();

        private IDepartmentService _departmentService;
        private IAgentService _agentService;

        public NotificationConnectionManager(
            IDepartmentService departmentService,
            IAgentService agentService
            )
        {
            _departmentService = departmentService;
            _agentService = agentService;
        }

        public void Connect(int siteId, int agentId, string connectionId)
        {
            AgentCacheItem user = GetAgent(siteId, agentId);
            _agentConnectionCache.Add(user, connectionId);
        }

        public void Reconnect(int siteId, int agentId, string connectionId)
        {
            if (!_agentConnectionCache.GetConnections(new AgentCacheItem(agentId, siteId)).Contains(connectionId))
            {
                Connect(siteId, agentId, connectionId);
            }
        }

        public void Disconnect(int siteId, int agentId, string connectionId)
        {
            _agentConnectionCache.Remove(new AgentCacheItem(agentId, siteId), connectionId);
        }

        public IList<string> GetConnections(int siteId, int agentId)
        {
            var agent = _agentConnectionCache.GetKeys().FirstOrDefault(t => t.SiteId == siteId && t.Id == agentId);
            if (agent == null)
            {
                return new List<string>();
            }
            if (agent.IsExpired)
            {
                agent = GetAgent(agent.SiteId, agent.Id);
                _agentConnectionCache.RefreshKey(agent);
            }
            return _agentConnectionCache.GetConnections(agent).ToList();
        }

        public IList<string> GetConnections(int siteId, int? agentAssignee, int? departmentAssigneee)
        {
            var result = new List<string>();

            var agents = _agentConnectionCache.GetKeys().Where(t => t.SiteId == siteId);
            if (!agents.Any())
            {
                return result;
            }

            foreach (var agent in agents)
            {
                var innerAgent = agent;
                if (agent.IsExpired)
                {
                    innerAgent = GetAgent(agent.SiteId, agent.Id);
                    _agentConnectionCache.RefreshKey(innerAgent);
                }

                // to do : check permission
                var agentConnections = _agentConnectionCache.GetConnections(innerAgent);
                result.AddRange(agentConnections);
            }

            return result.Distinct().ToList();
        }

        private AgentCacheItem GetAgent(int siteId, int agentId)
        {
            var user = new AgentCacheItem(agentId, siteId);
            UnitOfWorkManager.RunWithoutTransaction(siteId, () =>
            {
                var agent = _agentService.Find(agentId);
                if (agent != null)
                {
                    user.IfAdmin = agent.IfAdmin;
                }
                user.Departments = _departmentService.GetMyDepartmentIds(agentId);
                user.DepartmentMembers = _departmentService.GetMyDepartmentMembers(agentId);
            });

            return user;
        }
    }
}
