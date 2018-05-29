using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Infrastructure
{
    public static class TwitterHelper
    {
        public static string GetUserUrl(string screenName)
        {
            return $"https://twitter.com/{screenName}";
        }
    }
}
