namespace BotbrownWPF
{
    using BotbrownWPF.Views;
    using Castle.Core;
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Windsor;

    public class WpfInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.AddFacility<ViewActivatorFacility>();

            container.Register(
                Component.For<IWindsorContainer>().Instance(container),
                Component.For<IViewFactory>().ImplementedBy<WindsorViewFactory>(),
                Classes.FromThisAssembly().BasedOn<IView>()
                    .WithService.FromInterface()
                    .Configure(c => c
                        .LifeStyle.Is(LifestyleType.Transient)),
                Classes.FromThisAssembly().BasedOn<IViewModel>()
                    .WithService.FromInterface()
                    .Configure(c => c.LifeStyle.Is(LifestyleType.Transient))
              );

            /*
            container.Register(Component.For<MainWindow>().
                );*/

            container.Register(Classes.FromThisAssembly().InNamespace("BotbrownWPF", true)
                //.Unless(type => type == typeof(LoggingInterceptor))
                .WithServiceAllInterfaces()
                //.Configure(x => x.Interceptors<LoggingInterceptor>())
                );
        }
    }
}
