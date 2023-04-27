namespace BotbrownWPF
{
    using Castle.Core;
    using Castle.MicroKernel;
    using Castle.MicroKernel.ComponentActivator;
    using Castle.MicroKernel.Context;
    using System.Linq;
    using System.Windows;

    public class WPFWindowActivator : DefaultComponentActivator
    {
        public WPFWindowActivator(ComponentModel model, IKernelInternal kernel, ComponentInstanceDelegate onCreation, ComponentInstanceDelegate onDestruction) 
            : base(model, kernel, onCreation, onDestruction)
        { }

        protected override object CreateInstance(CreationContext context, ConstructorCandidate constructor, object[] arguments)
        {
            var component = base.CreateInstance(context, constructor, arguments);
            AssignViewModel(component, arguments);
            return component;
        }

        private void AssignViewModel(object component, object[] arguments)
        {
            FrameworkElement frameworkElement = component as FrameworkElement;
            if (frameworkElement == null || arguments == null)
            {
                return;
            }

            var vm = arguments.Where(a => a is IViewModel).FirstOrDefault();
            if (vm != null)
            {
                frameworkElement.DataContext = vm;
            }
        }
    }
}