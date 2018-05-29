using Social.WebApi.App_Start;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;
using Social.WebApi.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using Microsoft.Owin.Extensions;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using Social.WebApi.Core;
using Microsoft.Owin.Cors;
using Framework.Core.Json;
using Social.Infrastructure;
using System.Web.Http;

[assembly: OwinStartup(typeof(Startup))]

namespace Social.WebApi.App_Start
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //OAuthAuthorizationServerOptions oAuthServerOptions = new OAuthAuthorizationServerOptions()
            //{
            //    AllowInsecureHttp = true,
            //    TokenEndpointPath = new PathString("/token"),
            //    AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(30),
            //    Provider = new AuthorizationServerProvider(new TestClientStore()),
            //    RefreshTokenProvider = new RefreshTokenProvider(new TestRefreshTokenStore())
            //};

            //app.UseOAuthAuthorizationServer(oAuthServerOptions);
            app.UseJwtBearerAuthentication(AccountsController.JwtBearerOptions);

            var settings = new JsonSerializerSettings();
            settings.ContractResolver = new SignalRContractResolver();
            var serializer = JsonSerializer.Create(settings);
            serializer.Converters.Add(new UnitDateTimeConverter());
            GlobalHost.DependencyResolver.Register(typeof(JsonSerializer), () => serializer);
            var dependencyResolver = IocContainer.CreateDepenencyResolver();
            GlobalHost.DependencyResolver.Register(typeof(INotificationConnectionManager), () => dependencyResolver.Resolve<INotificationConnectionManager>());

            //var hubConfiguration = new HubConfiguration();
            //hubConfiguration.EnableDetailedErrors = true;
            //app.MapSignalR(hubConfiguration);

            // Branch the pipeline here for requests that start with "/signalr"
            app.Map("/signalr", map =>
            {
                // Setup the CORS middleware to run before SignalR.
                // By default this will allow all origins. You can 
                // configure the set of origins and/or http verbs by
                // providing a cors options with a different policy.
                map.UseCors(CorsOptions.AllowAll);
                var hubConfiguration = new HubConfiguration
                {
                    // You can enable JSONP by uncommenting line below.
                    // JSONP requests are insecure but some older browsers (and some
                    // versions of IE) require JSONP to work cross domain
                    // EnableJSONP = true
                };
                // Run the SignalR pipeline. We're not using MapSignalR
                // since this branch already runs under the "/signalr"
                // path.
                map.RunSignalR(hubConfiguration);
            });

            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);



            app.Use((context, next) =>
            {
                var httpContext = context.Get<HttpContextBase>(typeof(HttpContextBase).FullName);
                httpContext.SetSessionStateBehavior(SessionStateBehavior.Required);
                return next();
            });
            app.UseStageMarker(PipelineStage.MapHandler);
        }
    }
}