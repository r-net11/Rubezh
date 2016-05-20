using System;
using System.Linq;

namespace RubezhService
{
	class PollingPage : ConsolePageBase
	{
		public override string Name
		{
			get { return "F5-Полинг"; }
		}

		public override void Draw(int left, int top, int width, int height)
		{
			Console.BackgroundColor = ColorTheme.BackgroundColor;
			Console.ForegroundColor = ColorTheme.ForegroundColor;
			ConsoleHelper.WriteLine(left, top, width);
			DrawTableLine(left, top + 1, width, "IP адрес (Тип клиента)", "ID", "Первый", "Индекс");
			DrawTableLine(left, top + 2, width, "Имя пользователя", "", "Последний", "");
			ConsoleHelper.WriteLine(left, top + 3, width, String.Format(" {0} ", new String('-', width - 2)));
			ConsoleHelper.WriteLine(left, top + 4, width);
			var items = PollingPresenter.PollingItems
				.Skip(Math.Max(0, PollingPresenter.PollingItems.Count - (height - 7) / 3))
				.Take(Math.Min(PollingPresenter.PollingItems.Count, (height - 7) / 3)).ToList();
			var count = items.Count;
			for (int i = 0; i < count; i++)
			{
				DrawTableLine(left, top + 5 + i * 3, width,
					String.Format("{0} ({1})", items[i].IpAddress, items[i].ClientType),
					items[i].UID.ToString().Substring(0, 14),
					items[i].FirstPollTime.ToString(),
					items[i].CallbackIndex.ToString());
				DrawTableLine(left, top + 6 + i * 3, width,
					items[i].UserName,
					items[i].UID.ToString().Substring(14, 11) + "...",
					items[i].LastPollTime.ToString(),
					"");
				ConsoleHelper.WriteLine(left, top + 7 + i * 3, width);
			}
			for (int i = count * 3 + 5; i < height; i++)
				ConsoleHelper.WriteLine(left, top + i, width);

			EndDraw();
		}

		static void DrawTableLine(int left, int top, int width, string client, string uid, string firstLast, string index)
		{
			ConsoleHelper.WriteLine(left, top, width, String.Format(" {0} {1} {2} {3}",
					client.PadRight(33),
					uid.PadRight(15),
					firstLast.PadRight(20),
					index));
		}
	}
}
