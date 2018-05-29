using Social.Domain.DomainServices;
using Social.Domain.DomainServices.Facebook;
using Social.Infrastructure.Facebook;
using System.Threading.Tasks;
using Xunit;

namespace Social.IntegrationTest
{
    public class FacebookServiceTest : TestBase
    {
        [Fact]
        public async Task ShouldProcessData_WhenNewMessageCreated()
        {
            string rawData = "{\"entry\": [{\"changes\": [{\"field\": \"conversations\", \"value\": {\"thread_id\": \"t_mid.$cAAdZrm4k4UZh9X1vd1bxDgkg7Bo9\", \"page_id\": 1974003879498745}}], \"id\": \"1974003879498745\", \"time\": 1498785109}], \"object\": \"page\"}";

            var data = Newtonsoft.Json.JsonConvert.DeserializeObject<FbHookData>(rawData);
            IWebHookService facebookService = DependencyResolver.Resolve<IWebHookService>();
            await facebookService.ProcessWebHookData(data);
        }

        [Fact]
        public async Task ShouldProcessData_WhenPostCreated()
        {
            string rawData = "{\"entry\": [{\"changes\": [{\"field\": \"feed\", \"value\": {\"item\": \"post\", \"sender_name\": \"test\", \"sender_id\": \"121361878431739\", \"post_id\": \"1974003879498745_2015978795301253\", \"verb\": \"add\", \"created_time\": 1499675332, \"is_hidden\": false, \"message\": \"hi\"}}], \"id\": \"1974003879498745\", \"time\": 1499675333}], \"object\": \"page\"}";

            var data = Newtonsoft.Json.JsonConvert.DeserializeObject<FbHookData>(rawData);
            IWebHookService facebookService = DependencyResolver.Resolve<IWebHookService>();
            await facebookService.ProcessWebHookData(data);
        }

        [Fact]
        public async Task ShouldProcessData_WhenPostRemoved()
        {
            string rawData = "{ \"entry\": [{\"changes\": [{\"field\": \"feed\", \"value\": {\"verb\": \"remove\", \"item\": \"post\", \"sender_name\": \"test\", \"sender_id\": \"121361878431739\", \"post_id\": \"1974003879498745_2015978795301253\", \"recipient_id\": \"1974003879498745\", \"created_time\": 1499134045}}], \"id\": \"1974003879498745\", \"time\": 1499678628}], \"object\": \"page\"}";

            var data = Newtonsoft.Json.JsonConvert.DeserializeObject<FbHookData>(rawData);
            IWebHookService facebookService = DependencyResolver.Resolve<IWebHookService>();
            await facebookService.ProcessWebHookData(data);
        }

        [Fact]
        public async Task ShouldProcessData_WhenNewPhotoPost()
        {
            string rawData = "{\"entry\": [{\"changes\": [{\"field\": \"feed\", \"value\": {\"photo_id\": \"152819685285958\", \"item\": \"photo\", \"sender_name\": \"test\", \"sender_id\": \"121361878431739\", \"post_id\": \"1974003879498745_2015985455300587\", \"verb\": \"add\", \"link\": \"https://scontent.xx.fbcdn.net/v/t31.0-8/19956231_152819685285958_6092590179113745862_o.jpg?oh=f941d75af589ab07e2121cc9f3856efd&oe=59C91EB0\", \"published\": 1, \"created_time\": 1499680930, \"message\": \"this is a picture\"}}], \"id\": \"1974003879498745\", \"time\": 1499680949}], \"object\": \"page\"}";

            var data = Newtonsoft.Json.JsonConvert.DeserializeObject<FbHookData>(rawData);
            IWebHookService facebookService = DependencyResolver.Resolve<IWebHookService>();
            await facebookService.ProcessWebHookData(data);
        }

        [Fact]
        public async Task ShouldProcessData_WhenNewComment()
        {
            string rawData = "{\"entry\": [{\"changes\": [{\"field\": \"feed\", \"value\": {\"item\": \"comment\", \"sender_name\": \"test\", \"comment_id\": \"2015985455300587_2015999091965890\", \"sender_id\": \"121361878431739\", \"post_id\": \"1974003879498745_2015985455300587\", \"verb\": \"add\", \"parent_id\": \"1974003879498745_2015985455300587\", \"created_time\": 1499683257, \"message\": \"AAAA\"}}], \"id\": \"1974003879498745\", \"time\": 1499683259}], \"object\": \"page\"}";

            var data = Newtonsoft.Json.JsonConvert.DeserializeObject<FbHookData>(rawData);
            IWebHookService facebookService = DependencyResolver.Resolve<IWebHookService>();
            await facebookService.ProcessWebHookData(data);
        }


        [Fact]
        public async Task ShouldProcessData_WhenNewImageComment()
        {
            string rawData = "{\"entry\": [{\"changes\": [{\"field\": \"feed\", \"value\": {\"item\": \"comment\", \"sender_name\": \"test\", \"photo\": \"https://scontent.xx.fbcdn.net/v/t31.0-8/19956008_153078818593378_7165855581157346431_o.jpg?oh=f289baeb504d8867c3a4bef5aea0a321&oe=59CC9CD5\", \"comment_id\": \"2015985455300587_2016354585263674\", \"sender_id\": \"121361878431739\", \"post_id\": \"1974003879498745_2015985455300587\", \"verb\": \"add\", \"parent_id\": \"1974003879498745_2015985455300587\", \"created_time\": 1499741801, \"message\": \"picture comment\"}}], \"id\": \"1974003879498745\", \"time\": 1499741802}], \"object\": \"page\"}";

            var data = Newtonsoft.Json.JsonConvert.DeserializeObject<FbHookData>(rawData);
            IWebHookService facebookService = DependencyResolver.Resolve<IWebHookService>();
            await facebookService.ProcessWebHookData(data);
        }

        [Fact]
        public async Task ShouldProcessData_WhenRemoveComment()
        {
            string rawData = "{ \"entry\": [{\"changes\": [{\"field\": \"feed\", \"value\": {\"parent_id\": \"1974003879498745_2015985455300587\", \"sender_name\": \"Vivi Xu\", \"comment_id\": \"2015985455300587_2016354585263674\", \"sender_id\": \"121361878431739\", \"post_id\": \"1974003879498745_2015985455300587\", \"verb\": \"remove\", \"item\": \"comment\", \"created_time\": 1499742819}}], \"id\": \"1974003879498745\", \"time\": 1499742821}], \"object\": \"page\"}";

            var data = Newtonsoft.Json.JsonConvert.DeserializeObject<FbHookData>(rawData);
            IWebHookService facebookService = DependencyResolver.Resolve<IWebHookService>();
            await facebookService.ProcessWebHookData(data);
        }
    }
}