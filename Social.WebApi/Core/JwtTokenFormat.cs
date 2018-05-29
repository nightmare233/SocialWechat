using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Web;

namespace Social.WebApi.Core
{
    public class JwtTokenFormat : ISecureDataFormat<AuthenticationTicket>
    {
        private TimeSpan _tokenExpires;
        public JwtTokenFormat(TimeSpan tokenExpire)
        {
            _tokenExpires = tokenExpire;
        }

        public string SignatureAlgorithm
        {
            get { return "http://www.w3.org/2001/04/xmldsig-more#hmac-sha256"; }
        }

        public string DigestAlgorithm
        {
            get { return "http://www.w3.org/2001/04/xmlenc#sha256"; }
        }

        public string Protect(AuthenticationTicket data)
        {
            if (data == null) throw new ArgumentNullException("data");

            var issuer = "SocialIntegration";
            var audience = "all";
            var key = Convert.FromBase64String("a2FzamRmbGtqYXNkbGtmamFza2RmamFrc2Rhc2RmYXNkZmFzZGY=");
            var now = DateTime.UtcNow;
            var expires = now.Add(_tokenExpires);
            var signingCredentials = new SigningCredentials(
                                        new InMemorySymmetricSecurityKey(key),
                                        SignatureAlgorithm,
                                        DigestAlgorithm);
            var token = new JwtSecurityToken(issuer, audience, data.Identity.Claims,
                                             now, expires, signingCredentials);


            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public AuthenticationTicket Unprotect(string protectedText)
        {
            throw new NotImplementedException();
        }
    }
}