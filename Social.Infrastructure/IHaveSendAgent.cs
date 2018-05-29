using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Infrastructure
{
    public interface IHaveSendAgent
    {
        int? SendAgentId { get; set; }
        string SendAgentName { get; set; }
    }
}
