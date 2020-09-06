namespace BotBrown.Workers
{
    using Microsoft.Owin.FileSystems;
    using Microsoft.Owin.StaticFiles;
    using Owin;

    public class WebserverStartup
    {
        public void Configuration(IAppBuilder app)
        {
            IFileSystem fileSystem = new EmbeddedResourceFileSystem("BotBrown.www.dist");

            app.UseFileServer(new FileServerOptions
            {
                FileSystem = fileSystem,
                EnableDirectoryBrowsing = true
            });

#if DEBUG
            app.UseErrorPage();
#endif
            //app.UseWelcomePage("/");
        }
    }
}