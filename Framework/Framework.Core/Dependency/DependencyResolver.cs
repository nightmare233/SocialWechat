using Castle.MicroKernel.Lifestyle;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Framework.Core.UnitOfWork;
using Framework.Core.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Core
{
    public class DependencyResolver : IDependencyResolver
    {
        public IWindsorContainer IocContainer { get; private set; }

        public DependencyResolver()
        {
            IocContainer = new WindsorContainer();

            //Register self!
            IocContainer.Register(
                Component.For<DependencyResolver, IDependencyResolver>().UsingFactoryMethod(() => this).LifestyleSingleton(),

                Component.For(typeof(IDomainService<>)).ImplementedBy(typeof(DomainService<>)).LifestyleTransient(),
                Component.For(typeof(IRepository<,>)).ImplementedBy(typeof(EfRepository<,>)).LifestyleTransient(),
                Component.For(typeof(IRepository<,,>)).ImplementedBy(typeof(EfRepository<,,>)).LifestyleTransient(),
                Component.For<IUnitOfWorkManager>().ImplementedBy<UnitOfWorkManager>().LifestyleTransient(),
                Component.For<IUnitOfWork>().ImplementedBy<UnitOfWork.UnitOfWork>().LifestyleTransient(),
                Component.For<ITransactionStrategy>().ImplementedBy<TransactionStrategy>().LifestyleTransient(),
                Component.For<IDbContextResolver>().ImplementedBy<DefaultDbContextResolver>().LifestyleTransient(),
                Component.For<IConnectionStringResolver>().ImplementedBy<DefaultConnectionStringResolver>().LifestyleTransient(),
                Component.For<IUserSessionProvider>().ImplementedBy<DefaultUserSessionProvider>().LifestyleTransient(),
                Component.For<ICurrentUnitOfWorkProvider>().ImplementedBy<CurrentUnitOfWorkProvider>().LifestyleTransient()
                );
        }

        public void RegisterTransient<TService, TImpl>()
            where TService : class
            where TImpl : TService
        {
            IocContainer.Register(Component.For<TService>().ImplementedBy<TImpl>().LifestyleTransient());
        }

        public void RegisterTransient<TService>() where TService : class
        {
            IocContainer.Register(Component.For<TService>().LifestyleTransient());
        }

        public void RegisterTransient<TService>(TService instance)
            where TService : class
        {
            IocContainer.Register(Component.For<TService>().Instance(instance).LifestyleTransient());
        }

        public void Install(params IWindsorInstaller[] installers)
        {
            IocContainer.Install(installers);
        }

        public void RegisterAssemblyByConvention(Assembly assembly)
        {
            IocContainer.Register(
              Classes.FromAssembly(assembly)
                  .IncludeNonPublicTypes()
                  .BasedOn<ITransient>()
                  .If(type => !type.GetTypeInfo().IsGenericTypeDefinition)
                  .WithService.Self()
                  .WithService.AllInterfaces()
                  .Configure(configurer => configurer.Named(Guid.NewGuid().ToString())) //this is not a good idea, but we have to add these code to make container works fine with Obfuscator.
                  .LifestyleTransient(),
               Classes.FromAssembly(assembly)
                  .BasedOn(typeof(ServiceBase))
                  .WithServiceAllInterfaces()
                  .Configure(configurer => configurer.Named(Guid.NewGuid().ToString())) //this is not a good idea, but we have to add these code to make container works fine with Obfuscator.
                  .LifestyleTransient(),
               Classes.FromAssembly(assembly)
                  .BasedOn(typeof(EfRepository<,>))
                  .WithServiceAllInterfaces()
                  .Configure(configurer => configurer.Named(Guid.NewGuid().ToString())) //this is not a good idea, but we have to add these code to make container works fine with Obfuscator.
                  .LifestyleTransient(),
               Classes.FromAssembly(assembly)
                  .BasedOn(typeof(EfRepository<,,>))
                  .WithServiceAllInterfaces()
                  .Configure(configurer => configurer.Named(Guid.NewGuid().ToString())) //this is not a good idea, but we have to add these code to make container works fine with Obfuscator.
                  .LifestyleTransient()
              );
        }

        public bool IsRegistered(Type type)
        {
            return IocContainer.Kernel.HasComponent(type);
        }

        public bool IsRegistered<TType>()
        {
            return IocContainer.Kernel.HasComponent(typeof(TType));
        }

        public T Resolve<T>()
        {
            return IocContainer.Resolve<T>();
        }

        public T Resolve<T>(Type type)
        {
            return (T)IocContainer.Resolve(type);
        }

        public T Resolve<T>(object argumentsAsAnonymousType)
        {
            return IocContainer.Resolve<T>(argumentsAsAnonymousType);
        }

        public object Resolve(Type type)
        {
            return IocContainer.Resolve(type);
        }

        public object Resolve(Type type, object argumentsAsAnonymousType)
        {
            return IocContainer.Resolve(type, argumentsAsAnonymousType);
        }

        public T[] ResolveAll<T>()
        {
            return IocContainer.ResolveAll<T>();
        }

        public T[] ResolveAll<T>(object argumentsAsAnonymousType)
        {
            return IocContainer.ResolveAll<T>(argumentsAsAnonymousType);
        }

        public object[] ResolveAll(Type type)
        {
            return IocContainer.ResolveAll(type).Cast<object>().ToArray();
        }

        public object[] ResolveAll(Type type, object argumentsAsAnonymousType)
        {
            return IocContainer.ResolveAll(type, argumentsAsAnonymousType).Cast<object>().ToArray();
        }

        public void Release(object obj)
        {
            IocContainer.Release(obj);
        }

        public IDisposable BeginScope()
        {
            return IocContainer.Kernel.BeginScope();
        }

        public void Dispose()
        {
            IocContainer.Dispose();
        }
    }
}
