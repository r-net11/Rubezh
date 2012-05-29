using System;
using System.Windows.Input;
using Infrastructure.Common.MessageBox;

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