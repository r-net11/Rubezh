using System;

namespace RubezhService
{
	static class ConsoleHelper
	{
		public static void WriteLine(int left, int top, int width, object obj = null)
		{
			Console.SetCursorPosition(left, top);
			if (obj == null)
			{
				Console.Write(new String(' ', width));
			}
			else
			{
				var stringObj = obj.ToString();
				if (stringObj.Length > width)
					stringObj = stringObj.Substring(0, width);
				Console.Write(stringObj);
				WriteToEnd(width);
			}
		}
		
		public static void WriteToEnd(int width)
		{
			var length = width - Console.CursorLeft;
			if (length > 0)
				Console.Write(new String(' ', length));
		}

		public static void Write(int width)
		{
			Console.Write(new String(' ', width));
		}
	}
}
