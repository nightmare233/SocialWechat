using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Infrastructure.Facebook
{
    public class FbHookData
    {
        public string Object { get; set; }
        public List<FbHookDataEntry> Entry { get; set; }
    }
}
