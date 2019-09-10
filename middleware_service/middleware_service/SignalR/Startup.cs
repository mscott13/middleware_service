using Microsoft.AspNet.SignalR;
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
            var hubConfig = new HubConfiguration();
            hubConfig.EnableDetailedErrors = true;

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "info/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            
            config.MapHttpAttributeRoutes();
            config.Formatters.Remove(config.Formatters.XmlFormatter);
            appBuilder.UseCors(CorsOptions.AllowAll);
            appBuilder.UseWebApi(config);
            appBuilder.MapSignalR(hubConfig);
        }
    }
}
