using Infrastructure.Common;
using Common;
using System;

namespace FiresecService.Infrastructure
{
	public static class UserDialogService
	{
		public static bool ShowModalWindow(IDialogContent model)
		{
			try
			{
				var dialog = new DialogWindow();
				dialog.SetContent(model);

				bool? result = dialog.ShowDialog();
				if (result == null)
					return false;

				return (bool)result;
			}
			catch(Exception e)
			{
				Logger.Error(e, "Исключение при вызове UserDialogService.ShowModalWindow");
				return false;
			}
		}
	}
}
