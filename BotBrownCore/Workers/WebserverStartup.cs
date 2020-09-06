namespace BotBrown.Workers
{
    using Microsoft.Owin.FileSystems;
    using Microsoft.Owin.StaticFiles;
    using Owin;

    public class WebserverStartup
    {
        public void Configuration(IAppBuilder app)
        {
            //IFileSystem fileSystem = new EmbeddedResourceFileSystem("BotBrown.www");
            IFileSystem fileSystem = new PhysicalFileSystem(@"E:\DiscontinuedCoding\botbrown\BotBrownCore\www"); // TODO

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