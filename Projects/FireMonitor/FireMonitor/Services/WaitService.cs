using System;
using Infrastructure.Common.MessageBox;
using Infrastructure;
using System.Windows.Input;

namespace FireMonitor
{
	public class WaitService : IWaitService
	{
		public bool Execute(Action action)
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