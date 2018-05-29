using Framework.Core;
using Moq;
using Social.Domain.DomainServices;
using Social.Domain.Entities;
using Social.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.UnitTest
{
    public static class FakeServices
    {
        public static IAgentService MakeAgentService()
        {
            return new AgentService(null);
        }

        public static IDepartmentService MakeDepartmentService()
        {
            return new DepartmentService(new Mock<IRepository<Department>>().Object,
                new Mock<ICpanelConfigOptionRepositiory>().Object);
        }
    }
}
