using BotbrownWPF.Views;

namespace BotbrownWPF
{
    public interface IViewFactory
    {
        T CreateView<T>()
            where T : IView;
    }
}