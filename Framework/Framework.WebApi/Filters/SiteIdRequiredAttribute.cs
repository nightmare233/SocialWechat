using Framework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Framework.WebApi
{
    public class SiteIdRequiredAttribute : AuthorizationFilterAttribute
    {
        public override Task OnAuthorizationAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                if (!actionContext.ControllerContext.Controller.GetType().HasAttribute<IgnoreSiteIdAttribute>())
                {
                    actionContext.Request.CheckSiteId();
                }
            });
        }
    }
}
