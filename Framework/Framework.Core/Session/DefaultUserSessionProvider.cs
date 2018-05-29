using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Core
{
    public class DefaultUserSessionProvider : IUserSessionProvider, ITransient
    {
        public int GetUserId()
        {
            //int currentOperatorId = Comm.Comm100.Framework.Http.SessionHelper.GetSessionValue_CurrentOperatorId(siteId);
            return 1;
        }
    }
}
