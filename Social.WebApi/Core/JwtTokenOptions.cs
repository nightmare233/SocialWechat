using Microsoft.Owin.Security.Jwt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Social.WebApi.Core
{
    public class JwtTokenOptions : JwtBearerAuthenticationOptions
    {
        public JwtTokenOptions()
        {
            var issuer = "SocialIntegration";
            var audience = "all";
            var key = Convert.FromBase64String("a2FzamRmbGtqYXNkbGtmamFza2RmamFrc2Rhc2RmYXNkZmFzZGY=");

            AllowedAudiences = new[] { audience };
            IssuerSecurityTokenProviders = new[] { new SymmetricKeyIssuerSecurityTokenProvider(issuer, key) };

            Provider = new TokenProvider();
        }
    }
}