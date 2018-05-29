using Microsoft.Owin.Security.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Owin.Security.OAuth;

namespace Social.WebApi.Core
{
    public class TokenProvider : OAuthBearerAuthenticationProvider
    {
        public override Task RequestToken(OAuthRequestTokenContext context)
        {
            var value = context.Request.Query.Get("access_token");

            if (!string.IsNullOrEmpty(value))
            {
                context.Token = value;
            }

            return base.RequestToken(context);
        }

        public override Task ValidateIdentity(OAuthValidateIdentityContext context)
        {
            return base.ValidateIdentity(context);
        }
    }
}