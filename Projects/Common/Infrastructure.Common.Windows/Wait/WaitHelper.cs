using System;
using System.Windows.Input;
using Common;
using Infrastructure.Common.Windows.Windows;

namespace Infrastructure.Common.Windows
{
	public static class WaitHelper
	{
		public static bool Execute(Action action)
		{
			try
			{
				Mouse.SetCursor(Cursors.Wait);
				action();
				return true;
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове WaitHelper.Execute");
				MessageBoxService.ShowException(e);
			}
			finally
			{
				Mouse.SetCursor(Cursors.Arrow);
			}
			return true;
		}
	}
}