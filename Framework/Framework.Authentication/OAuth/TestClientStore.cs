using Framework.Core.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Authentication.OAuth
{
    public class TestClientStore : IOAuthClientStore
    {
        private static List<OAuthClient> _clients = new List<OAuthClient>();

        static TestClientStore()
        {
            //just for testing
            _clients.Add(new OAuthClient
            {
                Id = "test_client",
                Secret = "123",
                Name = "Test Client",
                Active = true,
                ApplicationType = OAuthClientApplicationTypes.JavaScript,
                RefreshTokenLifeTime = 7200,
                AllowedOrigin = "*"
            });
        }

        public OAuthClient Find(string clientId)
        {
            return _clients.FirstOrDefault(t => t.Id == clientId && t.Active);
        }

        public OAuthClient Find(string clientId, string clientSecret)
        {
            return _clients.FirstOrDefault(t => t.Id == clientId && t.Secret == clientSecret && t.Active);
        }
    }
}
