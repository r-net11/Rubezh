using System;
using System.Threading;

namespace Infrastructure.Common
{
	public static class TimeoutOperation
	{
		static AutoResetEvent _autoResetEvent = new AutoResetEvent(false);

		public static bool Execute(Action operation, TimeSpan timeout)
		{
			bool result = false;
			var thread = new Thread(() => 
			{
				try
				{
					operation();
					result = true;
				}
				catch
				{
				}
				finally
				{
					_autoResetEvent.Set();
				}
			});
			thread.Start();
			_autoResetEvent.WaitOne(timeout);
			thread.Abort();
			return result;
		}
	}
}
