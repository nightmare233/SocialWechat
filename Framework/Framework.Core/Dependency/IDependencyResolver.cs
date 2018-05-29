using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Core
{
    public interface IDependencyResolver
    {
        void RegisterAssemblyByConvention(Assembly assembly);

        T Resolve<T>();

        T Resolve<T>(Type type);

        T Resolve<T>(object argumentsAsAnonymousType);

        object Resolve(Type type);

        object Resolve(Type type, object argumentsAsAnonymousType);

        T[] ResolveAll<T>();

        T[] ResolveAll<T>(object argumentsAsAnonymousType);

        object[] ResolveAll(Type type);

        object[] ResolveAll(Type type, object argumentsAsAnonymousType);

        void Release(object obj);

        IDisposable BeginScope();

        bool IsRegistered(Type type);

        bool IsRegistered<T>();

        void RegisterTransient<TService>(TService instance) where TService : class;

        void RegisterTransient<TService, TImpl>() where TService : class where TImpl : TService;

        void RegisterTransient<TService>() where TService : class;
    }
}
