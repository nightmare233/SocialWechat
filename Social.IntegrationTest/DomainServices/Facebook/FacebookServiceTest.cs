using Social.Domain.DomainServices;
using Social.Domain.Entities;
using Social.Infrastructure.Facebook;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static Social.Infrastructure.Facebook.FacebookWebHookClient;

namespace Social.IntegrationTest
{
    public class FacebookServiceTest : TestBase
    {
        [Fact]
        public async Task ShouldProcessMessageData()
        {
            string rawData = "{\"entry\": [{\"changes\": [{\"field\": \"conversations\", \"value\": {\"thread_id\": \"t_mid.$cAAdZrm4k4UZh9X1vd1bxDgkg7Bo9\", \"page_id\": 1974003879498745}}], \"id\": \"1974003879498745\", \"time\": 1498785109}], \"object\": \"page\"}";

            var data = Newtonsoft.Json.JsonConvert.DeserializeObject<FbHookData>(rawData);
            IFacebookService facebookService = DependencyResolver.Resolve<IFacebookService>();
            await facebookService.ProcessWebHookData(data);
        }
    }
}