using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Infrastructure.Enum
{
    public enum ConversationLogType : short
    {
        ChangeAgentAssignee = 1,
        ChangeDepartmentAssignee = 2,
        ChangeStatus = 3,
        ChangePriority = 4,
        ChangeNote = 5,
        ChangeSubject = 6
    }
}
