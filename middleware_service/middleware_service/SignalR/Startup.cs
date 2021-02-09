using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Cors;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            const string rootFolder = ".";
            var fileSystem = new PhysicalFileSystem(rootFolder);
            var options = new FileServerOptions
            {
                EnableDefaultFiles = true,
                FileSystem = fileSystem
            };
            

            config.MapHttpAttributeRoutes();
            config.Formatters.Remove(config.Formatters.XmlFormatter);
            appBuilder.UseCors(CorsOptions.AllowAll);
            appBuilder.UseWebApi(config);
            appBuilder.MapSignalR(hubConfig);
            appBuilder.UseFileServer(options);
        }
        
    }
}
