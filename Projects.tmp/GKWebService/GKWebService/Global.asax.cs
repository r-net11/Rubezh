using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using GKWebService.DataProviders;
using GKWebService.DataProviders.Plan;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Mvc;
using Owin;

namespace GKWebService
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            Bootstrapper.Run();
            //PlansDataProvider.Instance.LoadPlans();
        }
    }

    
}
