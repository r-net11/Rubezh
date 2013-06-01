using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace SettingsModule.ViewModels
{
	public class SettingsViewModel : ViewPartViewModel
	{
		public ThemeViewModel ThemeContext { get; set; }
		public ConvertationViewModel ConvertationViewModel { get; set; }

		public void Initialize()
		{
			ThemeContext = new ThemeViewModel();
			ConvertationViewModel = new ConvertationViewModel();
			ShowSettingsCommand = new RelayCommand(OnShowSettings);
		}

		public RelayCommand ShowSettingsCommand { get; private set; }
		void OnShowSettings()
		{
			if (FiresecManager.CheckPermission(PermissionType.Adm_SetNewConfig))
			{
				var gloobalSettingsViewModel = new GloobalSettingsViewModel();
				DialogService.ShowModalWindow(gloobalSettingsViewModel);
			}
			else
			{
				MessageBoxService.ShowError("У Вас нет прав для изменения настроек");
			}
		}
	}
}