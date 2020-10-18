namespace BotbrownWPF
{
    using BotbrownWPF.Views;
    using Castle.MicroKernel;
    using Castle.Windsor;

    public class WindsorViewFactory : IViewFactory
    {
        private readonly IWindsorContainer container;

        public WindsorViewFactory(IWindsorContainer container)
        {
            this.container = container;
        }

        public T CreateView<T>() where T : IView
        {
            return container.Resolve<T>();
        }

        public T CreateView<T>(object argumentsAsAnonymousType)
          where T : IView
        {
            return container.Resolve<T>(Arguments.FromProperties(argumentsAsAnonymousType));
        }
    }
}
