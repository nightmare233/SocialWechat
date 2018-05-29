using Framework.Core;
using Social.Domain.Entities;
using Social.Domain.Entities.General;
using Social.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Domain.DomainServices
{
    public interface IAgentService
    {
        string GetDisplayName(int? id);
        void FillCreatedByName(IEnumerable<IHaveCreatedBy> list);
        int[] GetMatchedStatusAgents(int status);
        IList<Agent> FindAll();
        Agent Find(int id);
        IList<Agent> Find(IEnumerable<int> ids);
    }

    public class AgentService : ServiceBase, ITransient, IAgentService
    {
        private IRepository<User> _userRepo;

        public AgentService(IRepository<User> userRepo)
        {
            _userRepo = userRepo;
        }

        public IList<Agent> FindAll()
        {
            var agents = _userRepo.FindAll()
                .Where(t => t.UserType == 1 && t.IfActive && t.IfDeleted == false)
                .Select(t => new Agent { Id = t.Id, Name = t.Name, IfAdmin = t.IfAdmin }).ToList();

            return agents.OrderBy(t => t.Name).ToList();
            //var agents = new List<Agent>
            //{
            //    new Agent { Id=1,Name="Test Agent 1"},
            //    new Agent { Id=2,Name="Test Agent 2"},
            //    new Agent { Id=3,Name="Test Agent 3"},
            //    new Agent { Id=4,Name="Test Agent 4"},
            //    new Agent { Id=5,Name="Test Agent 5"},
            //    new Agent { Id=6,Name="Test Agent 6"},
            //    new Agent { Id=7,Name="Test Agent 7"},
            //    new Agent { Id=8,Name="Test Agent 8"},
            //    new Agent { Id=9,Name="Test Agent 9"},
            //    new Agent { Id=10,Name="Test Agent 10"},
            //};

            //return agents;
        }

        public Agent Find(int id)
        {
            return FindAll().FirstOrDefault(t => t.Id == id);
        }

        public IList<Agent> Find(IEnumerable<int> ids)
        {
            if (ids == null)
            {
                return new List<Agent>();
            }

            return FindAll().Where(t => ids.Contains(t.Id)).ToList();
        }

        public string GetDisplayName(int? id)
        {
            if (id == null)
            {
                return "N/A";
            }

            var agent = Find(id.Value);
            return agent == null ? "N/A" : agent.Name;
        }

        //public void FillAgentName(IEnumerable<IHaveSendAgent> list)
        //{
        //    List<int> agentIds = list.Where(t => t.SendAgentId != null).Select(t => t.SendAgentId.Value).Distinct().ToList();
        //    if (agentIds.Any())
        //    {
        //        var agents = FindAll().ToList();
        //        foreach (var item in list)
        //        {
        //            var agent = agents.FirstOrDefault(t => t.Id == item.SendAgentId);
        //            if (agent != null)
        //            {
        //                item.SendAgentName = agent.Name;
        //            }
        //        }
        //    }
        //}

        public void FillCreatedByName(IEnumerable<IHaveCreatedBy> list)
        {
            List<int> agentIds = list.Select(t => t.CreatedBy).Distinct().ToList();
            if (agentIds.Any())
            {
                var agents = FindAll().ToList();
                foreach (var item in list)
                {
                    var agent = agents.FirstOrDefault(t => t.Id == item.CreatedBy);
                    if (agent != null)
                    {
                        item.CreatedByName = agent.Name;
                    }
                }
            }
        }

        public int[] GetMatchedStatusAgents(int status)
        {
            return new int[] { };

        }

        public bool ChechAgentStatus(int id, int status)
        {
            return false;
        }
    }
}
