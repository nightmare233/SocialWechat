using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Framework.Core;
using Framework.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Social.WebApi.Core
{
    public class ControllersInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
            Classes.FromThisAssembly()
                .Pick().If(t => t.Name.EndsWith("Controller"))
                .Configure(configurer => configurer.Named(configurer.Implementation.Name))
                .LifestylePerWebRequest()
                );
        }
    }
}