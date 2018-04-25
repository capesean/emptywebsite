using System.Web;
using System.Web.Mvc;

namespace WEB
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new LogExceptionFilterAttribute());
            filters.Add(new HandleErrorAttribute());
        }
    }

    public class LogExceptionFilterAttribute : FilterAttribute, IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            Utilities.ErrorLogger.Log(filterContext.Exception, HttpContext.Current.Request, filterContext.HttpContext.Request.Url.ToString(), filterContext.HttpContext.User.Identity.Name);
        }
    }
}
