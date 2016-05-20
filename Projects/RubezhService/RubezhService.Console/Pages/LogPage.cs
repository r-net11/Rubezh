
using System;
namespace RubezhService
{
	class LogPage : ConsolePageBase
	{
		public override string Name
		{
			get { return "F2-Лог"; }
		}

		public override void Draw(int left, int top, int width, int height)
		{
			Console.BackgroundColor = ColorTheme.BackgroundColor;
			ConsoleHelper.WriteLine(left, top, width);
			var count = LogPresenter.LogItems.Count;
			for (int i = 0; i < count; i++)
			{
				Console.ForegroundColor = LogPresenter.LogItems[i].IsError ? ColorTheme.ErrorForegroundColor : ColorTheme.ForegroundColor;
				ConsoleHelper.WriteLine(left, top + i + 1, width, String.Format(" [{0}]: {1}", LogPresenter.LogItems[i].DateTime, LogPresenter.LogItems[i].Message));
			}
			for (int i = count + 1; i < height; i++)
				ConsoleHelper.WriteLine(left, top + i, width);

			EndDraw();
		}
	}
}
