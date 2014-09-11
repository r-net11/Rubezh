using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Common.Theme;
using Infrastructure.Common.Windows.ViewModels;

namespace SettingsModule.ViewModels
{
	public class ThemeViewModel : BaseViewModel
	{
		public ThemeViewModel()
		{
			Themes = Enum.GetValues(typeof(Theme)).Cast<Theme>().ToList();
			if (ThemeHelper.CurrentTheme != null)
				SelectedTheme = (Theme)Enum.Parse(typeof(Theme), ThemeHelper.CurrentTheme);
		}

		Theme _selectedTheme;
		public Theme SelectedTheme
		{
			get { return _selectedTheme; }
			set
			{
				_selectedTheme = value;
				ThemeHelper.SetThemeIntoRegister(_selectedTheme);
				ThemeHelper.LoadThemeFromRegister();
				OnPropertyChanged(() => SelectedTheme);
			}
		}
		public List<Theme> Themes { get; private set; }
	}
}