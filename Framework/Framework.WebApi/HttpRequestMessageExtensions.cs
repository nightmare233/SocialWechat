using Framework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Framework.WebApi
{
    public static class HttpRequestMessageExtensions
    {
        public static int GetSiteId(this HttpRequestMessage request)
        {
            var query = HttpUtility.ParseQueryString(request.RequestUri.Query);
            int siteId = 0;
            int.TryParse(query["siteId"], out siteId);
            return siteId;
        }

        public static void CheckSiteId(this HttpRequestMessage request)
        {
            var query = HttpUtility.ParseQueryString(request.RequestUri.Query);
            int siteId = 0;
            int.TryParse(query["siteId"], out siteId);
            if (siteId <= 0)
            {
                throw new ExceptionWithCode(HttpStatusCode.BadRequest, 40000, "Site id is required.");
            }

            //if(!SiteProcess.IfExistsSiteId(siteId))
            //{

            //}
        }
    }
}
