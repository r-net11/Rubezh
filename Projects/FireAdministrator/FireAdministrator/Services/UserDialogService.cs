using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Windows;
using Infrastructure;
using Infrastructure.Common;

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
            ActiveWindows.Remove((DialogWindow) sender);
        }

        void OnSaveSizeViewClosed(object sender, System.EventArgs e)
        {
            DialogWindow dialogWindow = sender as DialogWindow;
            var viewModel = dialogWindow.ViewModel;
            var typeName = viewModel.GetType().Name;

            var configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            configuration.AppSettings.Settings.Remove(typeName + "_Width");
            configuration.AppSettings.Settings.Remove(typeName + "_Height");
            configuration.AppSettings.Settings.Remove(typeName + "_Left");
            configuration.AppSettings.Settings.Remove(typeName + "_Top");
            configuration.AppSettings.Settings.Add(typeName + "_Width", dialogWindow.Width.ToString());
            configuration.AppSettings.Settings.Add(typeName + "_Height", dialogWindow.Height.ToString());
            configuration.AppSettings.Settings.Add(typeName + "_Left", dialogWindow.Left.ToString());
            configuration.AppSettings.Settings.Add(typeName + "_Top", dialogWindow.Top.ToString());
            configuration.Save();
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
                var dialogWindow = new DialogWindow();

                try
                {
                    dialogWindow.Owner = App.Current.MainWindow;
                }
                catch
                {
                    dialogWindow.ShowInTaskbar = true;
                }

                dialogWindow.SetContent(model);

                var viewModel = dialogWindow.ViewModel;
                var isSaveSize = viewModel.GetType().GetCustomAttributes(true).Any(x => x is SaveSizeAttribute);
                if (isSaveSize)
                {
                    var typeName = viewModel.GetType().Name;

                    try
                    {
                        var configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                        string stringWidth = configuration.AppSettings.Settings[typeName + "_Width"].Value;
                        string stringHeight = ConfigurationManager.AppSettings[typeName + "_Height"];
                        string stringLeft = ConfigurationManager.AppSettings[typeName + "_Left"];
                        string stringTop = ConfigurationManager.AppSettings[typeName + "_Top"];

                        dialogWindow.Width = double.Parse(stringWidth);
                        dialogWindow.Height = double.Parse(stringHeight);
                        dialogWindow.Left = double.Parse(stringLeft);
                        dialogWindow.Top = double.Parse(stringTop);
                    }
                    catch (Exception) { ;}

                    dialogWindow.Closed += new EventHandler(OnSaveSizeViewClosed);
                }

                bool? result = dialogWindow.ShowDialog();
                if (result == null)
                {
                    return false;
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