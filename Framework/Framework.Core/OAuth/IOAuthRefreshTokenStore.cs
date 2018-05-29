using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Core.OAuth
{
    public interface IOAuthRefreshTokenStore
    {
        void Add(OAuthRefreshToken refreshToken);
        void Delete(string refreshTokenId);
        OAuthRefreshToken Find(string refreshTokenId);
    }
}
