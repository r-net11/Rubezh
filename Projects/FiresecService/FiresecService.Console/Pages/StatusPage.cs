
using System;
namespace FiresecService
{
	class StatusPage : ConsolePageBase
	{
		public override string Name
		{
			get { return "F3-Статус"; }
		}

		public override void Draw(int left, int top, int width, int height)
		{
			Console.BackgroundColor = ColorTheme.BackgroundColor;
			Console.ForegroundColor = ColorTheme.ForegroundColor;
			ConsoleHelper.WriteLine(left, top, width);
			ConsoleHelper.WriteLine(left, top + 1, width, " Локальный адрес сервера: " + StatusPresenter.LocalAddress);
			ConsoleHelper.WriteLine(left, top + 2, width);
			ConsoleHelper.WriteLine(left, top + 3, width, " Удаленный адрес сервера: " + StatusPresenter.RemoteAddress);

			for (int i = 4; i < height; i++)
				ConsoleHelper.WriteLine(left, top + i, width);

			EndDraw();
		}
	}
}
