using System;
using System.Windows.Input;
using Common;
using Infrastructure.Common.Windows;

namespace Infrastructure.Common
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