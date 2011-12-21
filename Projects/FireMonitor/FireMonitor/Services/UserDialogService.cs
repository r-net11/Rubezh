using System.Collections.Generic;
using System.Linq;
using System.Windows;
using DevicesModule.ViewModels;
using Infrastructure.Common;

namespace FireMonitor
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

            var dialog = new DialogWindow()
            {
                Topmost = isTopMost,
                Name = name
            };
            dialog.SetContent(model);

            if (model is DeviceDetailsViewModel)
            {
                DeviceDetailsViewModel deviceDetailsViewModel = model as DeviceDetailsViewModel;
                var deviceUID = deviceDetailsViewModel.DeviceState.UID;

                DialogWindow existingWindow = ActiveWindows.FirstOrDefault(x => (x.ViewModel as DeviceDetailsViewModel).Device.UID == deviceUID);
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
            ActiveWindows.Remove((DialogWindow) sender);
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
            try
            {
                var dialog = new DialogWindow();
                dialog.SetContent(model);

                bool? result = dialog.ShowDialog();
                if (result == null)
                    return false;

                return (bool) result;
            }
            catch
            {
                throw;
            }
        }
    }
}