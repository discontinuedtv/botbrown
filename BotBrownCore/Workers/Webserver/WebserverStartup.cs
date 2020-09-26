namespace BotBrown.Workers.Webserver
{
    using Castle.Windsor;
    using Microsoft.Owin.FileSystems;
    using Microsoft.Owin.StaticFiles;
    using Owin;
    using System.Net.Http.Headers;
    using System.Web.Http;

    public class WebserverStartup
    {
        private IWindsorContainer container;

        public WebserverStartup(IWindsorContainer container)
        {
            this.container = container;
        }

        public void Configuration(IAppBuilder app)
        {
            container.Install(new ControllerInstaller(), new DefaultInstaller());


            IFileSystem fileSystem = new EmbeddedResourceFileSystem("BotBrown.www.dist");

            HttpConfiguration config = new HttpConfiguration();
            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
            config.DependencyResolver = new WindsorDependencyResolver(container);
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            app.UseFileServer(new FileServerOptions
            {
                FileSystem = fileSystem,
#if DEBUG
                EnableDirectoryBrowsing = true
#endif
            });

            app.UseWebApi(config);
#if DEBUG
            app.UseErrorPage();
#endif
            //app.UseWelcomePage("/");
        }
    }
}