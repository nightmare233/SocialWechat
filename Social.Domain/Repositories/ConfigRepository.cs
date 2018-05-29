using Framework.Core;
using Framework.Core.EntityFramework;
using Social.Domain.Core;
using Social.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Domain.Repositories
{
    public interface IConfigRepository : IRepository<GeneralDataContext, Config, int>
    {
    }
    public class ConfigRepository : EfRepository<GeneralDataContext, Config, int>, IConfigRepository
    {
    }
}
