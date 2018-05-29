using Social.WebApi.Models.Account;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using Social.Infrastructure;
using Social.WebApi.Core;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Web.Http;
using Framework.Core;
using Social.Application.AppServices;
using Framework.WebApi;
using Microsoft.Owin.Security.Jwt;

namespace Social.WebApi.Controllers
{
    [RoutePrefix("api/accounts")]
    public class AccountsController : ApiController
    {
        private IAgentAppService _agentAppService;

        public AccountsController(IAgentAppService agentAppService)
        {
            _agentAppService = agentAppService;
        }

        public static OAuthBearerAuthenticationOptions OAuthBearerOptions { get; private set; }
        public static JwtBearerAuthenticationOptions JwtBearerOptions { get; private set; }
        public static TimeSpan SessionDuration
        {
            get { return TimeSpan.FromMinutes(20); }
        }

        static AccountsController()
        {
            JwtBearerOptions = new JwtTokenOptions();
            OAuthBearerOptions = new OAuthBearerAuthenticationOptions();
            OAuthBearerOptions.Provider = new TokenProvider();
            OAuthBearerOptions.AccessTokenFormat = new JwtTokenFormat(TimeSpan.FromDays(7));
        }

        [HttpPost]
        [Route("token")]
        public string Authenticate(AuthenticationModel model)
        {
            var agent = _agentAppService.Find(model.UserId.Value);
            if (agent != null)
            {
                ClaimsIdentity claimsIdentity = new ClaimsIdentity("chat_server_session_identity");
                claimsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, model.UserId.ToString()));
                claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, agent.Name));
                claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, agent.IfAdmin ? "admin" : "agent"));
                claimsIdentity.AddClaim(new Claim(Comm100ClaimTypes.SiteId, Request.GetSiteId().ToString()));
                claimsIdentity.AddClaim(new Claim(Comm100ClaimTypes.SessionId, model.SessionId));
                var ticket = new AuthenticationTicket(claimsIdentity, new AuthenticationProperties());
                return OAuthBearerOptions.AccessTokenFormat.Protect(ticket);
            }
            return string.Empty;
        }

        [HttpGet]
        [Route("empty")]
        public IHttpActionResult Empty()
        {
            return Ok();
        }

        //[HttpGet]
        //[Route("me")]
        //public Me Me()
        //{
        //    ClaimsIdentity identity = Thread.CurrentPrincipal.Identity as ClaimsIdentity;
        //    var me = new Me
        //    {
        //        Id = identity.GetUserId<string>(),
        //        Name = identity.GetUserName(),
        //        Email = identity.Claims.FirstOrDefault(t => t.Type == ClaimTypes.Email).Value,
        //        SideId = identity.GetSideId()
        //    };

        //    return me;
        //}

        //[HttpPost]
        //[Route("token")]
        //public string Authenticate(AuthenticationModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return "Invalid request!";
        //    }

        //    //todo, validate user name and secret
        //    ClaimsIdentity claimsIdentity = TestClaimsIdentity.Create();
        //    var ticket = new AuthenticationTicket(claimsIdentity, new AuthenticationProperties());
        //    ticket.Properties.IssuedUtc = DateTime.UtcNow;
        //    ticket.Properties.ExpiresUtc = DateTime.UtcNow.Add(TimeSpan.FromMinutes(30));

        //    return OAuthBearerOptions.AccessTokenFormat.Protect(ticket);
        //}
    }
}
