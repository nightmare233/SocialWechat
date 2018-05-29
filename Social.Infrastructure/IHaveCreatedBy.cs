using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Infrastructure
{
    public interface IHaveCreatedBy
    {
        int CreatedBy { get; set; }
        string CreatedByName { get; set; }
    }
}
