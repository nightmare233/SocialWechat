using Framework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Framework.WebApi
{
    public class SessionAuthenticationHandler : DelegatingHandler
    {
        private IUserSessionProvider _userSessionProvider;

        public SessionAuthenticationHandler(IUserSessionProvider userSessionProvider)
        {
            _userSessionProvider = userSessionProvider;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            int siteId = request.GetSiteId();
            int currentOperatorId = _userSessionProvider.GetUserId();
            if (currentOperatorId > 0)
            {
                ClaimsIdentity cliamsIdentity = new ClaimsIdentity("SessionIdentity");
                cliamsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, currentOperatorId.ToString()));

                request.GetRequestContext().Principal = new ClaimsPrincipal(cliamsIdentity);
            }

            return base.SendAsync(request, cancellationToken);

        }
    }
}
