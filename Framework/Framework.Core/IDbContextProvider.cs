using Framework.Core;
using Framework.Core.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Core
{
    public class DefaultDbContextResolver : IDbContextResolver, ITransient
    {
        private IDependencyResolver _dependencyResolver;

        public DefaultDbContextResolver(IDependencyResolver dependencyResolver)
        {
            _dependencyResolver = dependencyResolver;
        }

        public TDbContext Resolve<TDbContext>(string nameOrConnectionString) where TDbContext : DbContext
        {
            return _dependencyResolver.Resolve<TDbContext>(new { nameOrConnectionString = nameOrConnectionString });
        }
    }

    public interface IDbContextResolver
    {
        TDbContext Resolve<TDbContext>(string nameOrConnectionString) where TDbContext : DbContext;
    }
}
