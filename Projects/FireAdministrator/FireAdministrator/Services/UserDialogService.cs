using System;
using System.Linq;
using System.Windows;
using Infrastructure.Common;
using System.Collections.Generic;

namespace FireAdministrator
{
    public class UserDialogService : IUserDialogService
    {
        List<DialogWindow> ActiveWindows = new List<DialogWindow>();

        public void ShowWindow(IDialogContent model, bool isTopMost = false, string name = "none")
        {
            if (name != "none")
            {
                var existingDialogWindow = ActiveWindows.FirstOrDefault(x => x.Name == name);
                if (existingDialogWindow != null)
                {
                    return;
                }
            }

            var dialogWindow = new DialogWindow()
            {
                Topmost = isTopMost,
                Name = name
            };
            dialogWindow.SetContent(model);

            if (name != "none")
            {
                dialogWindow.Closed += new System.EventHandler(dialog_Closed);
                ActiveWindows.Add(dialogWindow);
            }

            dialogWindow.Show();
        }

        void dialog_Closed(object sender, System.EventArgs e)
        {
            ActiveWindows.Remove((DialogWindow)sender);
        }

        public void HideWindow(string name)
        {
            var dialogWindow = ActiveWindows.FirstOrDefault(x => x.Name == name);
            if (dialogWindow != null)
            {
                dialogWindow.Visibility = Visibility.Collapsed;
            }
        }

        public void ResetWindow(string name)
        {
            var dialogWindow = ActiveWindows.FirstOrDefault(x => x.Name == name);
            if (dialogWindow != null)
            {
                dialogWindow.Visibility = Visibility.Visible;
            }
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
            catch (Exception)
            {
                throw;
            }
        }
    }
}