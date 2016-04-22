using System;

namespace FiresecService
{
	class GKPage : ConsolePageBase
	{
		public override string Name
		{
			get { return "F4-ГК"; }
		}

		public override void Draw(int left, int top, int width, int height)
		{
			Console.BackgroundColor = ColorTheme.BackgroundColor;
			Console.ForegroundColor = ColorTheme.ForegroundColor;
			ConsoleHelper.WriteLine(left, top, width);
			DrawTableLine(left, top + 1, width, "Время", "Адрес", "Название", "Прогресс");
			ConsoleHelper.WriteLine(left, top + 2, width, String.Format(" {0} ", new String('-', width - 2)));
			ConsoleHelper.WriteLine(left, top + 3, width);
			var list = GKPresenter.GetLifecycleItems(height - 6);
			for (int i = 0; i < list.Count; i++)
			{
				DrawTableLine(left, top + i + 4, width,
					list[i].DateTime.ToString(@"hh\:mm\:ss"),
					list[i].Address,
					list[i].GKLifecycleInfo.Name,
					list[i].GKLifecycleInfo.Progress);
			}
			for (int i = list.Count + 4; i < height; i++)
				ConsoleHelper.WriteLine(left, top + i, width);

			EndDraw();
		}

		static void DrawTableLine(int left, int top, int width, string time, string address, string name, string progress)
		{
			ConsoleHelper.WriteLine(left, top, width, String.Format(" {0} {1} {2} {3}",
					time.PadRight(10),
					address.PadRight(15),
					name.PadRight(34),
					progress));
		}
	}
}
