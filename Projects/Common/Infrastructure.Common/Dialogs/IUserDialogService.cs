using System.Windows;

namespace Infrastructure.Common
{
    public interface IUserDialogService
    {
        void ShowWindow(IDialogContent model, bool isTopMost = false, string name = "none");
        void HideWindow(string name);
        void ResetWindow(string name);
        bool ShowModalWindow(IDialogContent model);
    }
}