using System;

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
			catch (Exception e)
			{
				Logger.Error(e, Resources.Language.SafeContext.Execute_Exception);
			}
		}

		public static T Execute<T>(Func<T> action)
		{
			try
			{
				return action();
			}
			catch (Exception e)
			{
				Logger.Error(e, Resources.Language.SafeContext.ExecuteT_Exception);
				return default(T);
			}
		}
	}
}