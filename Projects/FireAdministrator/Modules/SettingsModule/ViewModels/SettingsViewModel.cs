using Infrastructure.Common.Windows.ViewModels;

namespace SettingsModule.ViewModels
{
	public class SettingsViewModel : ViewPartViewModel
	{
		public void Initialize()
		{
			ThemeContext = new ThemeViewModel();
			ConvertationViewModel = new ConvertationViewModel();
		}

		public ThemeViewModel ThemeContext { get; set; }
		public ConvertationViewModel ConvertationViewModel { get; set; }
	}
}