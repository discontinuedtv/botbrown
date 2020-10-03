namespace BotBrown.Workers.Webserver
{
    using Castle.Windsor;
    using Microsoft.Owin.Cors;
    using Microsoft.Owin.FileSystems;
    using Microsoft.Owin.StaticFiles;
    using Owin;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using System.Web.Cors;
    using System.Web.Http;
    using System.Web.Http.Cors;

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

            ConfigureCors(app);
            ConfigureFileServer(app);
            ConfigureHttp(app);

#if DEBUG
            app.UseErrorPage();
#endif
            //app.UseWelcomePage("/");
        }

        private static void ConfigureFileServer(IAppBuilder app)
        {
            IFileSystem fileSystem = new EmbeddedResourceFileSystem("BotBrown.www.dist");
            app.UseFileServer(new FileServerOptions
            {
                FileSystem = fileSystem,
#if DEBUG
                EnableDirectoryBrowsing = true
#endif
            });
        }

        private void ConfigureHttp(IAppBuilder app)
        {
            HttpConfiguration config = new HttpConfiguration();
            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
            config.DependencyResolver = new WindsorDependencyResolver(container);
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            app.UseWebApi(config);
        }

        private static void ConfigureCors(IAppBuilder app)
        {
            var corsPolicy = new CorsPolicy
            {
                AllowAnyMethod = true,
                AllowAnyHeader = true
            };

            var corsOptions = new CorsOptions
            {
                PolicyProvider = new CorsPolicyProvider
                {
                    PolicyResolver = context => Task.FromResult(corsPolicy)
                }
            };

            corsPolicy.Origins.Add("http://localhost:1234");

            app.UseCors(corsOptions);
        }
    }
}