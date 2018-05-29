using Social.Infrastructure.Facebook;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Social.WebApi.Controllers
{
    [RoutePrefix("api/test")]
    public class TestController : ApiController
    {
        [Route("facebook-post")]
        [HttpPost]
        public IHttpActionResult PublishFacebookPost(string pageId, [MaxLength(2000)] string message, string token)
        {
            FbClient.PublishPost(pageId, token, message);
            return Ok();
        }

        [Route("facebook-comment")]
        [HttpPost]
        public IHttpActionResult PublishFacebookComment([MaxLength(2000)]string message, string parentId, string token)
        {
            FbClient.PublishComment(token, parentId, message);
            return Ok();
        }
    }
}