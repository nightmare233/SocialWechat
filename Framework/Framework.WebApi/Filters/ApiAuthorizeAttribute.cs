using Framework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Framework.WebApi.Filters
{
    public class ApiAuthorizeAttribute : AuthorizationFilterAttribute
    {
        public override Task OnAuthorizationAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            int siteId = actionContext.Request.GetSiteId();

            var principal = actionContext.RequestContext.Principal as ClaimsPrincipal;

            if (principal == null || !principal.Identity.IsAuthenticated)
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, new ErrorInfo(400001, "unauthorized."));
            }

            return Task.FromResult<object>(null);
            //return base.OnAuthorizationAsync(actionContext, cancellationToken);
        }
    }
}
