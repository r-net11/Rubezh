using System.Windows;
using Infrastructure.Common;

namespace FireMonitor
{
    public class UserDialogService : IUserDialogService
    {
        public bool ShowWindow(IDialogContent model)
        {
            var dialog = new DialogWindow();
            dialog.SetContent(model);

            dialog.Show();
            return true;
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
                    Owner = parentWindow,
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