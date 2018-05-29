using Framework.Core.OAuth;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Framework.Authentication.OAuth
{
    public class TestRefreshTokenStore : IOAuthRefreshTokenStore
    {
        private static List<OAuthRefreshToken> RefreshTokens = new List<OAuthRefreshToken>();

        public void Add(OAuthRefreshToken refreshToken)
        {
            RefreshTokens.Add(refreshToken);
        }

        public OAuthRefreshToken Find(string refreshTokenId)
        {
            return RefreshTokens.FirstOrDefault(t => t.Id == refreshTokenId);
        }

        public void Delete(string refreshTokenId)
        {
            RefreshTokens.RemoveAll(t => t.Id == refreshTokenId);
        }
    }
}