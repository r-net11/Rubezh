using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using System.Windows;

namespace Controls.MessageBox
{
    internal static class UserDialogService
    {
        public static bool ShowModalWindow(IDialogContent model)
        {
            try
            {
                var dialogWindow = new DialogWindow();

                try
                {
                    dialogWindow.Owner = Application.Current.MainWindow;
                }
                catch
                {
                    dialogWindow.ShowInTaskbar = true;
                }

                dialogWindow.SetContent(model);

                bool? result = dialogWindow.ShowDialog();
                if (result == null)
                {
                    return false;
                }

                return (bool)result;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
