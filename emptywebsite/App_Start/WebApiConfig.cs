using Newtonsoft.Json.Serialization;
using System.Web.Http;

namespace WEB
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // error handling
            config.Filters.Add(new Utilities.ApiException());

            // Web API configuration and services
            config.Formatters.XmlFormatter.SupportedMediaTypes.Clear();
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            //config.Formatters.JsonFormatter.SerializerSettings.DateFormatString = "yyyy-MM-ddTHH:mm:ss";
            config.Formatters.JsonFormatter.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Local;

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
