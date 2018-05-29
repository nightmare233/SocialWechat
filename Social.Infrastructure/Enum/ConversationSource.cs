using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Infrastructure.Enum
{
    public enum ConversationSource : short
    {
        FacebookMessage = 1,
        FacebookVisitorPost = 2,
        FacebookWallPost = 3,
        TwitterTweet = 4,
        TwitterDirectMessage = 5
    }
}
