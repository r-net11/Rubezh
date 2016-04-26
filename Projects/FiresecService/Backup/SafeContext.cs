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
				Logger.Error(e, "Исключение при вызове SafeContext.Execute(Action action)");
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
				Logger.Error(e, "Исключение при вызове SafeContext.Execute<T>(Func<T> action)");
				return default(T);
			}
		}
	}
}