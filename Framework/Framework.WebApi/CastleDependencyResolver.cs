using Castle.MicroKernel;
using Castle.Windsor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Dependencies;

namespace Framework.WebApi
{
    public class CastleDependencyResolver : System.Web.Http.Dependencies.IDependencyResolver
    {
        private readonly IKernel _kernel;

        public CastleDependencyResolver(IKernel kernel)
        {
            this._kernel = kernel;
        }

        public IDependencyScope BeginScope()
        {
            return new CastleDependencyScope(_kernel);
        }

        public void Dispose()
        {
        }
        public object GetService(Type type)
        {
            return _kernel.HasComponent(type) ? _kernel.Resolve(type) : null;
        }

        public IEnumerable<object> GetServices(Type type)
        {
            return _kernel.ResolveAll(type).Cast<object>();
        }
    }
}