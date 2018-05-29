using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Core
{
    public class DefaultConnectionStringResolver : IConnectionStringResolver, ITransient
    {
        public string GetNameOrConnectionString(int? siteId)
        {
            if (siteId.HasValue)
            {
                var connectionString = ConfigurationManager.ConnectionStrings["SiteDataContext"].ConnectionString;
                return connectionString;
            }
            else
            {
                var connectionString = ConfigurationManager.ConnectionStrings["GeneralDataContext"].ConnectionString;
                return connectionString;
            }
        }
    }
}
