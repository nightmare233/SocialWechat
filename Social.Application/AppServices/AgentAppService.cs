using AutoMapper;
using AutoMapper.QueryableExtensions;
using Framework.Core;
using Social.Application.Dto;
using Social.Domain.DomainServices;
using Social.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Application.AppServices
{
    public interface IAgentAppService
    {
        List<AgentDto> FindAll();
        AgentDto Find(int id);
    }

    public class AgentAppService : AppService, IAgentAppService
    {
        private IAgentService _agentService;
        public AgentAppService(IAgentService agentService)
        {
            _agentService = agentService;
        }

        public List<AgentDto> FindAll()
        {
            return _agentService.FindAll().Select(t => Mapper.Map<AgentDto>(t)).ToList();
        }

        public AgentDto Find(int id)
        {
            return Mapper.Map<AgentDto>(_agentService.Find(id));
        }

    }
}
