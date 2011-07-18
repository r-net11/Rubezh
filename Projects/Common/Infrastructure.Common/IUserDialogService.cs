using System.Windows;

namespace Infrastructure.Common
{
    public interface IUserDialogService
    {
        bool ShowModalWindow(IDialogContent model);
        bool ShowModalWindow(IDialogContent model, Window parentWindow);
    }
}
