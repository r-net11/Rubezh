using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Common
{
	public static class SafeContext
	{
		public static void Execute(Action action)
		{
			try
			{
				action();
			}
			catch (Exception ex)
			{
				Logger.Error(ex);
				Debug.WriteLine(ex.ToString());
			}
		}
		public static T Execute<T>(Func<T> action)
		{
			try
			{
				return action();
			}
			catch (Exception ex)
			{
				Logger.Error(ex);
				Debug.WriteLine(ex.ToString());
				return default(T);
			}
		}
	}
}