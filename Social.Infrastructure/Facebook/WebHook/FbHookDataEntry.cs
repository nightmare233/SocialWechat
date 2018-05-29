using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Infrastructure.Facebook
{
    public class FbHookDataEntry
    {
        public string Id { get; set; }
        public long Time { get; set; }
        public List<FbHookMessage> Messaging { get; set; }
        public List<FbHookChange> Changes { get; set; }
    }
}
