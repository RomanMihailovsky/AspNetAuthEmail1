using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Globalization;
using System.Web.Http;

using Microsoft.AspNet.SignalR;
using ASPNetAuthEmail.Jobs;



namespace ASPNetAuthEmail
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {

            //RouteTable.Routes.MapHubs();

            AreaRegistration.RegisterAllAreas();

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);

            GlobalConfiguration.Configure(WebApiConfig.Register);

            RouteConfig.RegisterRoutes(RouteTable.Routes);

            BundleConfig.RegisterBundles(BundleTable.Bundles);

            CultureInfo cultureInfo = new CultureInfo("ru-RU");

            System.Threading.Thread.CurrentThread.CurrentCulture = cultureInfo;
            System.Threading.Thread.CurrentThread.CurrentUICulture = cultureInfo;

            //// запуск выполнения информатора
            //MsgScheduler.Start(HttpContext.Current);

            //MsgSender.Execute(p =>
            //{
            //    p.jobcontext.JobDataMap["context"] = HttpContext.Current; 
            //});

            //MsgSender.Execute(p =>
            //{
            //    p.jobcontext.JobDataMap["context"] = HttpContext.Current;
            //});


        }
    }
}
