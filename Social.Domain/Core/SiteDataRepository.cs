using Framework.Core;
using Framework.Core.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Domain.Core
{
    public class SiteDataRepository<TEntity> : EfRepository<SiteDataContext, TEntity>, IRepository<TEntity> where TEntity : Entity
    {
    }
}
