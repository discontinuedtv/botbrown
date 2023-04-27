using BotbrownWPF.Views;

namespace BotbrownWPF
{
    public interface IMainView : IView
    {
        bool? ShowDialog();
    }
}