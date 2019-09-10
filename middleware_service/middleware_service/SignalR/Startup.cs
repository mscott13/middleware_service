using Microsoft.Owin.Cors;
using Owin;
using System.Web.Http;

namespace middleware_service
{
    class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            HttpConfiguration config = new HttpConfiguration();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.MapHttpAttributeRoutes();
            config.Formatters.Remove(config.Formatters.XmlFormatter);
            appBuilder.UseWebApi(config);
            appBuilder.MapSignalR();
        }
    }
}
