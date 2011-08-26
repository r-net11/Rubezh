using System.Windows;

namespace Infrastructure.Common
{
    public interface IUserDialogService
    {
        bool ShowWindow(IDialogContent model);

        bool ShowModalWindow(IDialogContent model);

        bool ShowModalWindow(IDialogContent model, Window parentWindow);
    }
}