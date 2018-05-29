using Framework.Core;
using Framework.Core.Json;
using Framework.Core.UnitOfWork;
using Framework.WebApi;
using Framework.WebApi.Filters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Social.WebApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            IDependencyResolver resolver = config.DependencyResolver.GetService(typeof(IDependencyResolver)) as IDependencyResolver;

            config.MessageHandlers.Add(new SessionAuthenticationHandler(resolver.Resolve<IUserSessionProvider>()));

            // Web API 配置和服务
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver
                = new CamelCasePropertyNamesContractResolver();
            config.Formatters.JsonFormatter.UseDataContractJsonSerializer = false;
            config.Formatters.JsonFormatter.SerializerSettings.Converters.Add(new UnitDateTimeConverter());

            // Web API 路由
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Filters.Add(new SiteIdRequiredAttribute());
            config.Filters.Add(new InvalidParametersFilter());
            config.Filters.Add(new ValidateModelAttribute());
            config.Filters.Add(new UnitOfWorkFilter(resolver.Resolve<IUnitOfWorkManager>()));
            config.Filters.Add(new ApiExceptionFilterAttribute());
        }
    }
}
