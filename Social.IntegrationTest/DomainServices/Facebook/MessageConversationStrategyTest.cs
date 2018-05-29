using Social.Domain.DomainServices;
using Social.Domain.DomainServices.Facebook;
using Social.Domain.Entities;
using Social.Infrastructure.Facebook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Social.IntegrationTest.DomainServices.Facebook
{
    public class MessageConversationStrategyTest : TestBase
    {
        [Fact]
        public async Task ShouldGetLatestMessageFromConversation()
        {
            MessageStrategy facebookService = DependencyResolver.Resolve<MessageStrategy>();
            FbMessage message = await FacebookService.GetLastMessageFromConversationId
                (TestFacebookAccount.Token, "t_mid.$cAAdZrm4k4UZh9X1vd1bxDgkg7Bo9");

            Assert.NotNull(message);
            Assert.NotEmpty(message.Id);
            Assert.NotEmpty(message.SenderId);
            Assert.NotEmpty(message.ReceiverId);
            //Assert.NotNull(message.SenderEmail);
            //Assert.NotNull(message.FacebookConversationId);
        }
    }
}
