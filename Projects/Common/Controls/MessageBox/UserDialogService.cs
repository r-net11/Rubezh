using System;
using System.Windows;
using Common;
using Infrastructure.Common;

namespace Controls.MessageBox
{
    public static class UserDialogService
    {
		private static Window _activeModalWindow = null;

		public static bool ShowModalWindow(IDialogContent model)
		{
			return ShowModalWindow(model, null);
		}
 		public static bool ShowModalWindow(IDialogContent model, Action<DialogWindow> preshow)
        {
            try
            {
                var dialogWindow = new DialogWindow();

                try
                {
					dialogWindow.Owner = _activeModalWindow ?? Application.Current.MainWindow;
                }
                catch
                {
                    dialogWindow.ShowInTaskbar = true;
                }

                dialogWindow.SetContent(model);
				dialogWindow.WindowStartupLocation = dialogWindow.Owner == null ? WindowStartupLocation.CenterScreen : WindowStartupLocation.CenterOwner;

				if (preshow != null)
					preshow(dialogWindow);

				_activeModalWindow = dialogWindow;
				bool? result = dialogWindow.ShowDialog();
				_activeModalWindow = dialogWindow.Owner;
				if (result == null)
                {
                    return false;
                }

                return (bool)result;
            }
            catch (Exception e)
            {
                Logger.Error(e);
                throw;
            }
        }
    }
}