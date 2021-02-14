namespace BotbrownWPF
{
    using BotbrownWPF.Views;
    using Castle.MicroKernel.Facilities;
    using System.Linq;

    public class ViewActivatorFacility : AbstractFacility
    {
        protected override void Init()
        {
            Kernel.ComponentModelCreated +=
              Kernel_ComponentModelCreated;
        }

        void Kernel_ComponentModelCreated(Castle.Core.ComponentModel model)
        {
            var isView = typeof(IView).IsAssignableFrom(model.Services.First());
            if (!isView) return;

            if (model.CustomComponentActivator == null)
                model.CustomComponentActivator = typeof(WPFWindowActivator);
        }
    }
}
