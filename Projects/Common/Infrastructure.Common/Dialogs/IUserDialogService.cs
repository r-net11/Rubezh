using System.Windows;

namespace Infrastructure.Common
{
    public interface IUserDialogService
    {
        void ShowWindow(IDialogContent model, bool isTopMost = false);
        bool ShowModalWindow(IDialogContent model);
        bool ShowModalWindow(IDialogContent model, Window parentWindow);
    }
}