using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Social.Domain.Entities;
using Social.Domain.Entities.General;
using LinqKit;

namespace Social.Domain.DomainServices
{
    public class AgentAssigneeStatusExpression : OptionExpression
    {
        private IAgentService _agentService;
        public AgentAssigneeStatusExpression(IAgentService agentService) : base("Agent Assignee Status", "")
        {
            _agentService = agentService;
        }

        protected override object GetValue(string value)
        {
            return null;
        }

        protected override Expression<Func<Conversation, bool>> Is(FilterCondition condition)
        {
            int[] agentIds = _agentService.GetMatchedStatusAgents(int.Parse(condition.Value));
            return t => t.AgentId.HasValue && agentIds.Contains(t.AgentId.Value);
        }

        protected override Expression<Func<Conversation, bool>> IsNot(FilterCondition condition)
        {
            int[] agentIds = _agentService.GetMatchedStatusAgents(int.Parse(condition.Value));
            return t => t.AgentId.HasValue && !agentIds.Contains(t.AgentId.Value) || !t.AgentId.HasValue;
        }
    }
}
