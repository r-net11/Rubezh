using System;
using System.Linq;

namespace RubezhService
{
	class ConnectionsPage : ConsolePageBase
	{
		public override string Name
		{
			get { return "F1-Соединения"; }
		}

		public override void Draw(int left, int top, int width, int height)
		{
			Console.BackgroundColor = ColorTheme.BackgroundColor;
			Console.ForegroundColor = ColorTheme.ForegroundColor;
			ConsoleHelper.WriteLine(left, top, width);
			DrawTableLine(left, top + 1, width, "№", "Тип", "Адрес", "Пользователь");
			ConsoleHelper.WriteLine(left, top + 2, width, String.Format(" {0} ", new String('-', width - 2)));
			ConsoleHelper.WriteLine(left, top + 3, width);
			var list = ConnectionsPresenter.Connections.ToList();
			for (int i = 0; i < list.Count; i++)
			{
				DrawTableLine(left, top + i + 4, width,
					(i + 1).ToString() + ".",
					list[i].ClientType,
					list[i].IpAddress,
					list[i].FriendlyUserName);
			}
			for (int i = list.Count + 4; i < height; i++)
				ConsoleHelper.WriteLine(left, top + i, width);

			EndDraw();
		}

		static void DrawTableLine(int left, int top, int width, string n, string clientType, string ipAddress, string name)
		{
			ConsoleHelper.WriteLine(left, top, width, String.Format(" {0} {1} {2} {3}",
					n.PadRight(3),
					clientType.PadRight(18),
					ipAddress.PadRight(16),
					name));
		}
	}
}
