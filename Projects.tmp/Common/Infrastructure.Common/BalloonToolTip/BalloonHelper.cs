using System;
using System.Windows.Media;
using System.Windows.Threading;
using Common;

namespace Infrastructure.Common.BalloonTrayTip
{
	public class BalloonHelper
	{
		public static void ShowFromFiresec(string text)
		{
			Show("Глобал", text);
		}

		public static void ShowFromAdm(string text)
		{
			Show("Администратор", text);
		}

		public static void ShowFromMonitor(string text)
		{
			Show("Оперативная задача", text);
		}

		public static void ShowFromServer(string text)
		{
			Show("Сервер приложений", text);
		}

		public static void Show(string title, string text)
		{
			var foregroundColor = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.Blue;
			Console.WriteLine ("{0}: {1}", title, text);
			Console.ForegroundColor = foregroundColor;
		}
	}
}