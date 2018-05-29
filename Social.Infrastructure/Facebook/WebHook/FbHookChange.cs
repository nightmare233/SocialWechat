using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Infrastructure.Facebook
{
    public class FbHookChange
    {
        public string Field { get; set; }
        public FbHookChangeValue Value { get; set; }
    }
}
