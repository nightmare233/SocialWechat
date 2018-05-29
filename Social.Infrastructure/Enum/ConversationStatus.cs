using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Infrastructure.Enum
{
    public enum ConversationStatus : short
    {
        [Display(Name = "New")]
        New = 0,
        [Display(Name = "Pending Internal")]
        PendingInternal = 1,
        [Display(Name = "Pending External")]
        PendingExternal = 2,
        [Display(Name = "OnHold")]
        OnHold = 3,
        [Display(Name = "Closed")]
        Closed = 4
    }
}
