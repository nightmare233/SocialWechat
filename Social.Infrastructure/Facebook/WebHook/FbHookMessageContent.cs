using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Infrastructure.Facebook
{
    public class FbHookMessageContent
    {
        public string Mid { get; set; }
        public long Seq { get; set; }
        public string Text { get; set; }
    }
}
