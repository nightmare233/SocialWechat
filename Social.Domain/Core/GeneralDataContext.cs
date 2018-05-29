using Framework.Core;
using Framework.Core.EntityFramework;
using log4net;
using Social.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Domain.Core
{
    public class GeneralDataContext : DataContext, ITransient
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(SiteDataContext));

        public GeneralDataContext(string nameOrConnectionString) : base(nameOrConnectionString)
        {
            Database.SetInitializer<GeneralDataContext>(null);
            Database.Log = t => logger.Debug(t);
        }

        public virtual DbSet<SiteSocialAccount> Conversations { get; set; }
        public virtual DbSet<Config> Config { get; set; }       
    }
}
