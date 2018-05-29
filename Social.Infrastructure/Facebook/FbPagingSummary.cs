using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Infrastructure.Facebook
{
    public class FbPagingSummary
    {
        public string order { get; set; }
        public int total_count { get; set; }
        public bool can_comment { get; set; }
    }
}
