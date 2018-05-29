using Social.Application.AppServices;
using Social.Application.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Social.WebApi.Controllers
{
    /// <summary>
    /// api about agents.
    /// </summary>
    [RoutePrefix("api/agents")]
    public class AgentController : ApiController
    {
        private IAgentAppService _agentService;

        /// <summary>
        /// AgentController
        /// </summary>
        /// <param name="agentService"></param>
        public AgentController(IAgentAppService agentService)
        {
            _agentService = agentService;
        }

        /// <summary>
        /// Get agents.
        /// </summary>
        /// <returns></returns>
        [Route()]
        public List<AgentDto> GetAgents()
        {
            return _agentService.FindAll();
        }

        /// <summary>
        /// Get agent by id.
        /// </summary>
        /// <param name="id">agent id</param>
        /// <returns></returns>
        [Route("{id}", Name = "GetAgent")]
        public AgentDto GetAgent(int id)
        {
            return _agentService.Find(id);
        }
    }
}