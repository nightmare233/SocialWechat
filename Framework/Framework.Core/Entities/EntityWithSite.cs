using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Core
{
    public class EntityWithSite : Entity, IHaveSite
    {
        public int SiteId { get; set; }
    }
}
