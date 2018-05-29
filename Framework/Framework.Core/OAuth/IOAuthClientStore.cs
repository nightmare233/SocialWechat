using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Core.OAuth
{
    public interface IOAuthClientStore
    {
        OAuthClient Find(string clientId);
    }
}
