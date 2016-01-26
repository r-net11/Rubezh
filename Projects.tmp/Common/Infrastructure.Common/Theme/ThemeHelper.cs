using System;
using System.Windows;
using Common;

namespace Infrastructure.Common.Theme
{
	public class ThemeHelper
	{
		public static string CurrentTheme { get; private set; }

		public static void SetThemeIntoRegister(Theme selectedTheme)
		{
			RegistrySettingsHelper.SetString("Theme", selectedTheme.ToString());
		}

		public static void LoadThemeFromRegister()
		{
			try
			{
				CurrentTheme = RegistrySettingsHelper.GetString("Theme");
				if (String.IsNullOrEmpty(CurrentTheme))
				{
					CurrentTheme = "BlueTheme";
				}
				var themePath = "pack://application:,,,/Controls, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;component/Themes/" + CurrentTheme + ".xaml";

			}
			catch (Exception e)
			{
				CurrentTheme = "BlueTheme";
				var themePath = "pack://application:,,,/Controls, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;component/Themes/" + CurrentTheme + ".xaml";
				Logger.Error(e, "ThemeHelper.LoadThemeFromRegister");
			}
		}
	}
}