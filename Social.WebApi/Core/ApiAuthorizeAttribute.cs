using Framework.Core;
using Social.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Social.WebApi.Core
{
    public class ApiAuthorizeAttribute : AuthorizationFilterAttribute
    {
        public string Permissions { get; set; }

        public override Task OnAuthorizationAsync(HttpActionContext actionContext, System.Threading.CancellationToken cancellationToken)
        {

            var principal = actionContext.RequestContext.Principal as ClaimsPrincipal;

            if (principal == null || !principal.Identity.IsAuthenticated)
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, new ErrorInfo(0, "Please sign-in first."));
                return Task.FromResult<object>(null);
            }

            return Task.FromResult<object>(null);
        }
    }
}