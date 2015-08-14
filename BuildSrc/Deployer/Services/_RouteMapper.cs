using DotNetNuke.Web.Api;

namespace Build.DotNetNuke.Deployer.Services
{
    public class _RouteMapper : IServiceRouteMapper
    {
        public void RegisterRoutes(IMapRoute mapRouteManager)
        {
            mapRouteManager.MapHttpRoute("Deployer",
                routeName: "DefaultController",
                url: "{action}",
                defaults: new { controller = "AdminBase", action = "Index" },
                namespaces: new[] { "Build.DotNetNuke.Deployer.Services" }
                );

            mapRouteManager.MapHttpRoute("Deployer",
                routeName: "Default",
                url: "{controller}/{action}",
                defaults: new { controller = "AdminBase", action = "Index" },
                namespaces: new[] { "Build.DotNetNuke.Deployer.Services" }
                );
        }
    }
}