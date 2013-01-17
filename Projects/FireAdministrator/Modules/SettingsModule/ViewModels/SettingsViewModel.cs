using Infrastructure.Common.Windows.ViewModels;

namespace SettingsModule.ViewModels
{
	public class SettingsViewModel : ViewPartViewModel
	{
		public void Initialize()
		{
			ThemeContext = new ThemeViewModel();
			ModuleContext = new ModuleViewModel();
			ConvertationViewModel = new ConvertationViewModel();
			FSC2ViewModel = new FSC2ViewModel();
		}

		public ThemeViewModel ThemeContext { get; set; }
		public ModuleViewModel ModuleContext { get; set; }
		public ConvertationViewModel ConvertationViewModel { get; set; }
		public FSC2ViewModel FSC2ViewModel { get; set; }
	}
}