using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Infrastructure.Enum
{
    public enum MessageSource : short
    {
        FacebookMessage = 1,
        FacebookPost = 2,
        FacebookPostComment = 3,
        TwitterTypicalTweet = 4,
        TwitterQuoteTweet = 5,
        TwitterDirectMessage = 6,
        FacebookPostReplyComment = 7
    }
}
