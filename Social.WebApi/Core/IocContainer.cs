using Castle.Windsor;
using Framework.Core;
using Framework.WebApi;
using Social.Application;
using System.Reflection;
using System.Web.Http;

namespace Social.WebApi.Core
{
    public static class IocContainer
    {
        public static void Setup()
        {
            Framework.Core.DependencyResolver dependencyResolver = CreateDepenencyResolver();

            GlobalConfiguration.Configuration.DependencyResolver = new CastleDependencyResolver(dependencyResolver.IocContainer.Kernel);
        }

        public static DependencyResolver CreateDepenencyResolver()
        {
            Framework.Core.DependencyResolver dependencyResolver = new Framework.Core.DependencyResolver();
            dependencyResolver.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
            new ApplicationServicesRegistrar(dependencyResolver).RegisterServices();

            dependencyResolver.Install(
                new ControllersInstaller()
                );

            return dependencyResolver;
        }
    }
}