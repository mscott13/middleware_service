using Microsoft.Owin.Cors;
using Owin;
using System.Web.Http;

namespace middleware_service
{
    class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            HttpConfiguration config = new HttpConfiguration();
            config.Routes.MapHttpRoute(name: "DefaultApi", 
                routeTemplate: "{controller}/{id}",
                defaults: new { id = RouteParameter.Optional });

            app.UseCors(CorsOptions.AllowAll);
            app.MapSignalR();
        }
    }
}
