using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Infrastructure.Facebook
{
    public class FbHookMessageDelivery
    {
        public string Watermark { get; set; }
        public long Seq { get; set; }
        public List<string> Mids { get; set; }
    }
}
