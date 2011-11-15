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

        public void ShowWindow(IDialogContent model, bool isTopMost = false)
        {
            var dialog = new DialogWindow()
            {
                Topmost = isTopMost
            };
            dialog.SetContent(model);

            if (model is PlansModule.ViewModels.DevicesViewModel)
            {
                DialogWindow existingWindow = ActiveWindows.FirstOrDefault(x => x.ViewModel is PlansModule.ViewModels.DevicesViewModel);
                if (existingWindow != null)
                {
                    existingWindow.Activate();
                    return;
                }

                dialog.Closed += new System.EventHandler(dialog_Closed);
                ActiveWindows.Add(dialog);
            }

            dialog.Show();
        }

        void dialog_Closed(object sender, System.EventArgs e)
        {
            ActiveWindows.Remove((DialogWindow)sender);
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