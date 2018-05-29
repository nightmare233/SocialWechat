using Microsoft.AspNet.SignalR;
using Social.WebApi.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Social.WebApi.Controllers
{
    [RoutePrefix("api/filters")]
    public class NotificationTestController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}