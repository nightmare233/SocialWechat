using Framework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Social.WebApi.Core
{
    public class UserContext : IUserContext
    {
        public UserContext()
        {
        }

        public int UserId
        {
            get
            {
                int userId = 0;
                var userIdOrNull = Thread.CurrentPrincipal.Identity.GetUserId();
                if (userIdOrNull != null)
                {
                    userId = userIdOrNull.Value;
                }

                return userId;
            }
        }

        public int? SiteId
        {
            get
            {
                if (HttpContext.Current == null)
                {
                    return 0;
                }

                string siteIdStr = string.Empty;
                try
                {
                    siteIdStr = HttpContext.Current.Request["siteId"];
                }
                catch (HttpRequestValidationException ex)
                {
                    //todo 
                    //log
                    return 0;
                }
                int siteId;
                return int.TryParse(siteIdStr, out siteId) ? (int?)siteId : null;
            }
        }
    }
}
