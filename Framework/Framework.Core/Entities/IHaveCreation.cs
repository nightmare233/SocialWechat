using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Core
{
    public interface IHaveCreation : IHaveCreatedTime
    {
        int CreatedBy { get; set; }
    }

    public interface IHaveCreatedTime
    {
        DateTime CreatedTime { get; set; }
    }
}
