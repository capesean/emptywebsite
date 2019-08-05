﻿using System;
using System.Globalization;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace WEB
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            GlobalFilters.Filters.Add(new AntiForgeryTokenFilter());
        }

        public class AntiForgeryTokenFilter : FilterAttribute, IExceptionFilter
        {
            public void OnException(ExceptionContext filterContext)
            {
                if (filterContext.Exception.GetType() == typeof(HttpAntiForgeryException))
                {
                    filterContext.Result = new RedirectResult("/logout");
                    filterContext.ExceptionHandled = true;
                }
            }
        }

        protected void Application_BeginRequest()
        {
            // customise this according to your region
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-ZA");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-ZA");
            Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator = ".";
            Thread.CurrentThread.CurrentCulture.NumberFormat.NumberGroupSeparator = ",";
            Thread.CurrentThread.CurrentCulture.NumberFormat.PercentDecimalSeparator = ".";
            Thread.CurrentThread.CurrentCulture.NumberFormat.PercentGroupSeparator = ",";
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var httpContext = ((MvcApplication)sender).Context;

            var currentRouteData = RouteTable.Routes.GetRouteData(new HttpContextWrapper(httpContext));
            var currentController = " ";
            var currentAction = " ";

            if (currentRouteData != null)
            {
                if (currentRouteData.Values["controller"] != null &&
                    !string.IsNullOrEmpty(currentRouteData.Values["controller"].ToString()))
                {
                    currentController = currentRouteData.Values["controller"].ToString();
                }

                if (currentRouteData.Values["action"] != null &&
                    !string.IsNullOrEmpty(currentRouteData.Values["action"].ToString()))
                {
                    currentAction = currentRouteData.Values["action"].ToString();
                }
            }

            var ex = Server.GetLastError();

            if (ex != null)
            {
                Utilities.ErrorLogger.Log(ex, HttpContext.Current.Request, HttpContext.Current.Request.Url.ToString(), httpContext.User == null ? null : httpContext.User.Identity.Name);
            }

            //#if !DEBUG
            //if (!Request.IsLocal)
            //{
            //    var controller = new ErrorController();
            //    var routeData = new RouteData();
            //    var statusCode = 500;

            //    if (ex is HttpException)
            //    {
            //        var httpEx = ex as HttpException;
            //        statusCode = httpEx.GetHttpCode();
            //    }
            //    else if (ex is AuthenticationException)
            //    {
            //        statusCode = 403;
            //    }

            //    httpContext.ClearError();
            //    httpContext.Response.Clear();
            //    httpContext.Response.StatusCode = statusCode;
            //    httpContext.Response.TrySkipIisCustomErrors = true;
            //    routeData.Values["controller"] = "Error";
            //    routeData.Values["action"] = "Index";
            //    routeData.Values["id"] = statusCode;

            //    controller.ViewData.Model = new HandleErrorInfo(ex, currentController, currentAction);
            //    ((IController)controller).Execute(new RequestContext(new HttpContextWrapper(httpContext), routeData));
            //}
            //#endif
        }
    }
}
