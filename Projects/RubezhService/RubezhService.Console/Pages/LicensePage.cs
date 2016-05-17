using RubezhAPI;
using RubezhAPI.License;
using System;

namespace RubezhService
{
	class LicensePage : ConsolePageBase
	{
		public override string Name
		{
			get { return "F7-Лицензия"; }
		}

		int _left;
		int _top;
		int _width;
		int _height;

		public override void Draw(int left, int top, int width, int height)
		{
			_left = left;
			_top = top;
			_width = width;
			_height = height;
			Console.BackgroundColor = ColorTheme.BackgroundColor;
			Console.ForegroundColor = ColorTheme.ForegroundColor;
			ConsoleHelper.WriteLine(left, top, width);

			Console.Write(" Статус лицензии: ");
			Console.ForegroundColor = LicensePresenter.LicenseInfo.LicenseMode == LicenseMode.HasLicense ? ColorTheme.SuccessForegroundColor : ColorTheme.ErrorForegroundColor;
			Console.Write(LicensePresenter.LicenseInfo.LicenseMode.ToDescription());
			Console.ForegroundColor = ColorTheme.ForegroundColor;
			ConsoleHelper.WriteToEnd(width);

			ConsoleHelper.WriteLine(left, top + 2, width);
			if (LicensePresenter.LicenseInfo.LicenseMode == LicenseMode.NoLicense)
			{
				for (int i = 3; i <= 8; i++)
					ConsoleHelper.WriteLine(left, top + i, width);
			}
			else
			{
				ConsoleHelper.WriteLine(left, top + 3, width, String.Format(" GLOBAL Удаленное рабочее место (количество): {0}", LicensePresenter.LicenseInfo.RemoteClientsCount));
				ConsoleHelper.WriteLine(left, top + 4, width, String.Format(" GLOBAL Пожаротушение:                        {0}", LicensePresenter.LicenseInfo.HasFirefighting.ToYesNo()));
				ConsoleHelper.WriteLine(left, top + 5, width, String.Format(" GLOBAL Охрана:                               {0}", LicensePresenter.LicenseInfo.HasGuard.ToYesNo()));
				ConsoleHelper.WriteLine(left, top + 6, width, String.Format(" GLOBAL Доступ:                               {0}", LicensePresenter.LicenseInfo.HasSKD.ToYesNo()));
				ConsoleHelper.WriteLine(left, top + 7, width, String.Format(" GLOBAL Видео:                                {0}", LicensePresenter.LicenseInfo.HasVideo.ToYesNo()));
				ConsoleHelper.WriteLine(left, top + 8, width, String.Format(" GLOBAL OPC Сервер:                           {0}", LicensePresenter.LicenseInfo.HasOpcServer.ToYesNo()));
			}
			for (int i = 9; i < height - 6; i++)
				ConsoleHelper.WriteLine(left, top + i, width);
			ConsoleHelper.WriteLine(left, height + top - 6, width, String.Format(" Ключ: {0}", LicensePresenter.InitialKey));
			ConsoleHelper.WriteToEnd(width);

			var buttonText = "F8-Загрузить лицензию";

			ConsoleHelper.Write(1);
			Console.BackgroundColor = ColorTheme.ButtonBackgroundColor;
			ConsoleHelper.Write(buttonText.Length + 4);
			Console.BackgroundColor = ColorTheme.BackgroundColor;
			ConsoleHelper.WriteToEnd(width);

			ConsoleHelper.Write(1);
			Console.BackgroundColor = ColorTheme.ButtonBackgroundColor;
			Console.ForegroundColor = ColorTheme.ButtonForegroundColor;
			Console.Write(String.Format("  {0}  ", buttonText));
			Console.BackgroundColor = ColorTheme.BackgroundColor;
			ConsoleHelper.WriteToEnd(width);

			ConsoleHelper.Write(1);
			Console.BackgroundColor = ColorTheme.ButtonBackgroundColor;
			ConsoleHelper.Write(buttonText.Length + 4);
			Console.BackgroundColor = ColorTheme.BackgroundColor;
			ConsoleHelper.WriteToEnd(width);
			ConsoleHelper.WriteToEnd(width);

			EndDraw();
		}

		public override void OnKeyPressed(ConsoleKey key)
		{
			switch (key)
			{
				case ConsoleKey.F8:
					Console.BackgroundColor = ColorTheme.TextBoxBackgroundColor;
					Console.ForegroundColor = ColorTheme.TextBoxForegroundColor;
					Console.SetCursorPosition(_left, _height + _top - 4);
					for (int i = 0; i < 4; i++)
						ConsoleHelper.WriteToEnd(_width);
					EndDraw();
					Console.SetCursorPosition(_left, _height + _top - 4);
					Console.CursorVisible = true;
					Console.Write("Введите путь к файлу лицензии: ");
					var path = Console.ReadLine();
					Console.CursorVisible = false;
					var error = LicensePresenter.LoadLicense(path);
					if (!String.IsNullOrEmpty(error))
					{
						Console.SetCursorPosition(_left, _height + _top - 4);
						for (int i = 0; i < 4; i++)
							ConsoleHelper.WriteToEnd(_width);
						Console.SetCursorPosition(_left, _height + _top - 4);
						Console.Write(error);
						EndDraw();
						Console.ReadKey();
					}
					Draw(_left, _top, _width, _height);
					break;
			}
		}
	}
}
