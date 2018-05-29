using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Infrastructure.Facebook
{
    public class FbPaging
    {
        public FbCursors cursors { get; set; }
        public string next { get; set; }
        public string previous { get; set; }
    }
}
