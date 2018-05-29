using Social.Infrastructure.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Infrastructure
{
    public interface INotificationConnectionManager
    {
        void Connect(int siteId, int agentId, string connectionId);
        void Reconnect(int siteId, int agentId, string connectionId);
        void Disconnect(int siteId, int agentId, string connectionId);
        IList<string> GetConnections(int siteId, int? agentAssignee, int? departmentAssigneee);
        IList<string> GetConnections(int siteId, int agentId);
    }
}
