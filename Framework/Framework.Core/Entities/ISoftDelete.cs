using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Core
{
    public interface ISoftDelete
    {
        bool IsDeleted { get; set; }
    }
}
