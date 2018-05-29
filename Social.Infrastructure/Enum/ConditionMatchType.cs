using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Infrastructure.Enum
{
    public enum ConditionMatchType : short
    {
        Is = 1,
        IsNot = 2,
        IsMoreThan = 3,
        IsLessThan = 4,
        Contain = 5,
        NotContain = 6,
        Before = 7,
        After = 8,
        Between = 9,
    }
}
