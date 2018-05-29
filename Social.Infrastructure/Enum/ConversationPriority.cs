using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Infrastructure.Enum
{
    public enum ConversationPriority : short
    {
        [Display(Name = "Low")]
        Low = 0,

        [Display(Name = "Normal")]
        Normal = 1,

        [Display(Name = "High")]
        High = 2,

        [Display(Name = "Urgent")]
        Urgent = 3
    }
}
