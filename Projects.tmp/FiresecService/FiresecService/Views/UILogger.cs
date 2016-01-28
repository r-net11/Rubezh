using System;
namespace FiresecService.ViewModels
{
	public static class UILogger
	{
		public static void Log(string message, bool isError = false)
		{
			var foregroundColor = Console.ForegroundColor;
			Console.ForegroundColor = isError ? ConsoleColor.Red : ConsoleColor.Green;
			Console.WriteLine ("UILogger: {0}", message);
			Console.ForegroundColor = foregroundColor;
		}
	}
}