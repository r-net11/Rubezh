using System.Windows;
using Infrastructure.Common;

namespace FireMonitor
{
    public class UserDialogService : IUserDialogService
    {
        public void ShowWindow(IDialogContent model, bool isTopMost = false)
        {
            var dialog = new DialogWindow()
            {
                Topmost = isTopMost
            };
            dialog.SetContent(model);

            dialog.Show();
        }

        public bool ShowModalWindow(IDialogContent model)
        {
            return ShowModalWindow(model, Application.Current.MainWindow);
        }

        public bool ShowModalWindow(IDialogContent model, Window parentWindow)
        {
            try
            {
                var dialog = new DialogWindow
                {
                    //Owner = parentWindow
                };
                dialog.SetContent(model);

                bool? result = dialog.ShowDialog();
                if (result == null)
                {
                }

                return (bool) result;
            }
            catch
            {
                throw;
            }
        }
    }
}