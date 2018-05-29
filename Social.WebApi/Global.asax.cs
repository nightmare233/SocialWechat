using AutoMapper;
using log4net;
using Social.WebApi;
using Social.WebApi.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.SessionState;

namespace Social.WebApi
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(HttpApplication));

        protected void Application_Start()
        {
            log.Info("Application starting...");

            IocContainer.Setup();

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            Mapper.Initialize(cfg =>
                cfg.AddProfiles(new[] {
                    "Social.Application"
                })
            );

            log.Info("Application started...");
        }

        protected void Application_Error()
        {
            var raisedException = Server.GetLastError();
            if (raisedException != null)
            {
                log.Error(raisedException.Message, raisedException);
            }
        }

        //public override void Init()
        //{
        //    this.PostAuthenticateRequest += WebApiApplication_PostAuthenticateRequest;
        //    base.Init();
        //}

        //void WebApiApplication_PostAuthenticateRequest(object sender, EventArgs e)
        //{
        //    HttpContext.Current.SetSessionStateBehavior(
        //        SessionStateBehavior.Required);
        //}
    }
}
