using Castle.MicroKernel;
using Castle.MicroKernel.Lifestyle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Dependencies;

namespace Framework.WebApi
{
    public class CastleDependencyScope: IDependencyScope
    {
        private readonly IKernel _kernel;

        private readonly IDisposable disposable;

        public CastleDependencyScope(IKernel kernel)
        {
            this._kernel = kernel;
            disposable = _kernel.BeginScope();
        }

        public object GetService(Type type)
        {
            return _kernel.HasComponent(type) ? _kernel.Resolve(type) : null;
        }

        public IEnumerable<object> GetServices(Type type)
        {
            return _kernel.ResolveAll(type).Cast<object>();
        }

        public void Dispose()
        {
            disposable.Dispose();
        }
    }
}