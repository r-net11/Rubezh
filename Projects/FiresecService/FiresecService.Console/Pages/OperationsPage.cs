
using System;
namespace FiresecService
{
	class OperationsPage : ConsolePageBase
	{
		public override string Name
		{
			get { return "F6-Операции"; }
		}

		public override void Draw(int left, int top, int width, int height)
		{
			Console.BackgroundColor = ColorTheme.BackgroundColor;
			Console.ForegroundColor = ColorTheme.ForegroundColor;
			ConsoleHelper.WriteLine(left, top, width);
			var count = OperationsPresenter.Operations.Count;
			for (int i = 0; i < count; i++)
			{
				ConsoleHelper.WriteLine(left, top + i + 1, width, " " + OperationsPresenter.Operations[i].Name);
			}
			for (int i = count + 1; i < height; i++)
				ConsoleHelper.WriteLine(left, top + i, width);

			EndDraw();
		}
	}
}
