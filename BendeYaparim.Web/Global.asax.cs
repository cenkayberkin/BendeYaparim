using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Ninject;
using System.Web.Security;

namespace BendeYaparim.Web
{

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorWithELMAHAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("elmah.axd");
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("favicon.ico");

            routes.IgnoreRoute("{file}.php");

            routes.IgnoreRoute("{file}.txt");
            routes.IgnoreRoute("{file}.htm");
            routes.IgnoreRoute("{file}.html");

            routes.MapRoute(
            "CategoryAdvice", // Route name
              "Kategori-Tavsiye-et", // URL with parameters
                new { controller = "Category", action = "CategoryAdvice" } // Parameter defaults
            );

            routes.MapRoute(
            "UserProfile", // Route name
              "Kullanıcı/{Id}/", // URL with parameters
                new { controller = "User", action = "Profile", CategoryId = UrlParameter.Optional } // Parameter defaults
            );

            routes.MapRoute(
            "JobOffer", // Route name
              "Eleman-Arama/{CategoryId}/{CategoryName}/", // URL with parameters
                new { controller = "JobOffer", action = "Index", CategoryId = UrlParameter.Optional } // Parameter defaults
            );

            routes.MapRoute(
            "JobSeek", // Route name
              "İş-Arama/{CategoryId}/{CategoryName}/", // URL with parameters
                new { controller = "JobSeek", action = "Index", CategoryId = UrlParameter.Optional } // Parameter defaults
            );

            routes.MapRoute(
            "SeekSubCategories", // Route name
              "Kategoriler/İş-Arama-alt-kategorileri/{id}", // URL with parameters
                new { controller = "Category", action = "SeekSubCategories", id = UrlParameter.Optional } // Parameter defaults
            );

            routes.MapRoute(
            "OfferSubCategories", // Route name
              "Kategoriler/Eleman-Arama-alt-kategorileri/{id}", // URL with parameters
                new { controller = "Category", action = "OfferSubCategories", id = UrlParameter.Optional } // Parameter defaults
            );

            routes.MapRoute(
                "AccountRegister",
              "Site-Kayıt", // URL with parameters
              new { controller = "Account", action = "Register" } // Parameter defaults
            );

            routes.MapRoute(
                "AccountLogon",
              "Site-Girisi", // URL with parameters
              new { controller = "Account", action = "LogOn" } // Parameter defaults
            );

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

            routes.MapRoute(
                "Activate", "Account/Activate/{username}/{key}", new
                {
                    controller = "Account",
                    action = "Activate",
                    username = UrlParameter.Optional,
                    key = UrlParameter.Optional
                });

        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            HibernatingRhinos.Profiler.Appender.EntityFramework.EntityFrameworkProfiler.Initialize();
            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
        }
    }
}